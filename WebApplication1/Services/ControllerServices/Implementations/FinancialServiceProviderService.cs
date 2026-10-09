// Services/BankService.cs
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using System.Text.Json;
using VMS.Modules.Licenses.Core.Emails.EmailSenderService.Entities;
using WebApplication1.DAL.Repository;
using WebApplication1.DAL;
using WebApplication1.Domain.DTO;
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Enums;
using WebApplication1.Domain.Repository;
using WebApplication1.DTOs;
using WebApplication1.Repositories;
using WebApplication1.Services.Emails.EmailService.Entities;
using WebApplication1.Services.Emails.TemplateService.Enitities;
using WebApplication1.Services.Emails.EmailService.Queuer;
using Microsoft.Extensions.Options;
using WebApplication1.Services.TokenService;
using MimeKit.Encodings;
using Microsoft.EntityFrameworkCore;
using System.Runtime.ConstrainedExecution;
using static QRCoder.PayloadGenerator.SwissQrCode;
using HandlebarsDotNet;

namespace WebApplication1.Services.ControllerServices.Implementations
{
    public class FinancialServiceProviderService : IFinancialServiceProviderService
    {
        private readonly IFinancialServiceProviderRepository _servceProviderRepository;
        private readonly IFinancialServiceProviderContactPersonRepository _contactPersonRepository;
        private readonly ITransactionCodeRepository _transactionCodeRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICurrencyRepository _currencyRepository;
        private readonly ITransactionService _transactionService;

        //for transaction
        private readonly AppDbContext _dbContext;
        public FinancialServiceProviderService(
             IFinancialServiceProviderRepository bankRepository,
             IFinancialServiceProviderContactPersonRepository contactPersonRepository,
             ITransactionCodeRepository transactionCodeRepository,
             ILocationRepository locationRepository,
             IUserRepository userRepository,
              ICurrencyRepository currencyRepository,
             AppDbContext db,
             ITransactionService transactionService
            )
        {
            _servceProviderRepository = bankRepository;
            _contactPersonRepository = contactPersonRepository;
            _transactionCodeRepository = transactionCodeRepository;
            _locationRepository = locationRepository;
            _userRepository = userRepository;
            _currencyRepository = currencyRepository;
            _dbContext = db;
            _transactionService = transactionService;
        }

        public async Task<FinancialServiceProviderResponseDto> CreateBankAsync(CreateFinancialServiceProviderDto createDto)
        {
            // Validate unique code
            var transaction = await _dbContext.Database.BeginTransactionAsync();


            var validLocation = await _locationRepository.GetByIdAsync(createDto.LocationId);
            if (validLocation == null) { throw new Exception("Invalid shop submitted"); }

            var validType = Enum.IsDefined(typeof(FinancialServiceProviderType), createDto.Type);
            if (validType == false) throw new Exception("Invalid type detected");

            var user = await _userRepository.GetUserByRefreshTokenAsync();

            if(user == null)
            {
                throw new Exception("Invalid User");
            }

            try
            {
                var bank = FinancialServiceProvider.Create(
                Guid.NewGuid(),
                "",
                createDto.Name,
                createDto.Type,
                createDto.Address,
                createDto.LocationId,
                Guid.Parse(user.Id),
                DateTime.UtcNow,
                createDto.Status
            );

                await _servceProviderRepository.AddAsync(bank);

                // Create contact persons if any
                var contactPersons = new List<FinancialServiceProviderContactPerson>();
                if (createDto.ContactPersons != null && createDto.ContactPersons.Any())
                {
                    foreach (var contactPersonDto in createDto.ContactPersons)
                    {


                        var contactPerson = FinancialServiceProviderContactPerson.Create(
                            Guid.NewGuid(),
                             "",
                            contactPersonDto.FullName,
                            contactPersonDto.Email ?? "",
                            contactPersonDto.PhoneNumber,
                            bank.Id,
                            Guid.Parse(user.Id),
                            DateTime.UtcNow,
                            contactPersonDto.Location
                        );

                        contactPersons.Add(contactPerson);
                    }
                }

                if (contactPersons.Any()) await _contactPersonRepository.AddRangeAsync(contactPersons);

                await  _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();


                return new FinancialServiceProviderResponseDto
                {
                    Code = bank.Code,
                    Id = bank.Id,
                    Address = bank.Address,
                    Name = bank.Name,
                    Status = bank.GeneralStatus.ToString(),
                    ContactPersons = contactPersons.Select(x => new FinancialServiceProviderContactPersonResponseDto
                    {
                        Id = x.Id,
                        FinancialServiceProviderId = x.FinancialServiceProviderId,
                        Status = x.GeneralStatus.ToString(),
                        Code = x.Code,
                        FullName = x.FullName
                    }).ToList()
                };
            }
            catch(Exception ex)
            {
                await transaction.RollbackAsync();
                throw;
            } 
            

           
        }

        public async Task<FinancialServiceProviderResponseDto> UpdateBankAsync(Guid bankId, FinancialServiceProviderUpdateDto updateDto)
        {
            var user = await _userRepository.GetUserByRefreshTokenAsync();
            var bank = await _servceProviderRepository.GetByIdAsync(bankId);


            var transaction = await _dbContext.Database.BeginTransactionAsync();

            if (user == null)
            {
                throw new Exception($"Unauthorized.");
            }


            if (bank == null)
            {
                throw new Exception($"Financial service provider not found.");
            }

            if (updateDto.Status != null)
            {
                var isValidStatus = Enum.IsDefined(typeof(GeneralStatus), updateDto.Status);
                if (!isValidStatus) { throw new Exception("Invalid status detected"); }
            }

            if(updateDto.Type != null)
            {
                var isValidType = Enum.IsDefined(typeof(FinancialServiceProviderType), updateDto.Type);
                if (!isValidType) { throw new Exception("Invalid status detected"); }
            }
            


            var contactPersonsToUpdate = new List<FinancialServiceProviderContactPerson>();
            var contactPersonsToCreate = new List<FinancialServiceProviderContactPerson>();

            try
            {
                // Update bank basic info
                bank.Update(updateDto.Name ?? bank.Name, updateDto.Type ?? bank.Type, updateDto.Address, (GeneralStatus)updateDto.Status, Guid.Parse(user.Id));


                await _servceProviderRepository.Update(bank);

                // Handle contact persons updates if provided
                if (updateDto.ContactPersons != null && updateDto.ContactPersons.Any())
                {
                    var existingContactPersons = bank.ContactPersons.ToList();

                    for (int i = 0; i < updateDto.ContactPersons.Count; i++)
                    {
                        var contactPerson = existingContactPersons[i];
                        var updateData = updateDto.ContactPersons.Where(x => x.Id == existingContactPersons[i].Id).FirstOrDefault();

                        if (updateData != null)
                        {
                            contactPerson.Update(
                               updateData.FullName,
                               updateData.Email,
                               updateData.PhoneNumber,
                               updateData.Status,
                               Guid.Parse(user.Id),
                               updateDto.Address
                           );
                            contactPersonsToUpdate.Add(contactPerson);
                        }
                        else
                        {
                            var contactPersonToSave = FinancialServiceProviderContactPerson.Create(
                                Guid.NewGuid(),
                                 "",
                                updateDto.ContactPersons[i].FullName,
                                updateDto.ContactPersons[i].Email,
                                updateDto.ContactPersons[i].PhoneNumber,
                                bank.Id,
                                Guid.Parse(user.Id),
                                DateTime.UtcNow,
                                updateDto.Address
                            );


                        }
                    }
                }

                await _contactPersonRepository.UpdateRageAsync(contactPersonsToUpdate);

                await transaction.CommitAsync();
            } catch(Exception ex)
            {
                await transaction.RollbackAsync();
                throw;
            }

            return new FinancialServiceProviderResponseDto
            {
                Code = bank.Code,
                Id = bank.Id,
                Address = bank.Address,
                Name = bank.Name,
                Status = bank.GeneralStatus.ToString(),
                ContactPersons = contactPersonsToUpdate.Select(x => new FinancialServiceProviderContactPersonResponseDto
                {
                    Id = x.Id,
                    FinancialServiceProviderId = x.FinancialServiceProviderId,
                    Status = x.GeneralStatus.ToString(),
                    Code = x.Code,
                    FullName = x.FullName
                }).ToList()
            };
        }



        public async Task DeleteBankAsync(Guid bankId)
        {
            var user = await _userRepository.GetUserByRefreshTokenAsync();
            var bank = await _servceProviderRepository.GetByIdAsync(bankId);
            if (bank == null)
            {
                throw new Exception($"Financial service provider not found.");
            }

            // Soft delete bank
            bank.Update(bank.Name,bank.Type, bank.Address, GeneralStatus.SoftDeleted, Guid.Parse(user.Id));
            await _servceProviderRepository.Update(bank);

            //// Soft delete all contact persons
            //foreach (var contactPerson in bank.ContactPersons)
            //{
            //    contactPerson.Update(
            //        contactPerson.FullName,
            //        GeneralStatus.Inactive,
            //        bank.UpdatedBy
            //    );
            //    _contactPersonRepository.Update(contactPerson);
            //}

            //await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<FinancialServiceProviderDropdownDto>> GetBanksForDropdownAsync(Guid locationId, GeneralStatus? status)
        {
            var banks =  _servceProviderRepository.GetFinancialServiceProvidersByLocationAsync(locationId, null, status ?? GeneralStatus.Active);
            return  await banks.Select(x => new FinancialServiceProviderDropdownDto
            {
                Code = x.Code,
                Id = x.Id,
                Name = x.Name
            }).ToListAsync();
        }

        public async Task<IEnumerable<FinancialServiceProviderContactPersonResponseDto>> GetAllContactPersonsAsync(Guid bankId, string? filter, GeneralStatus? status)
        {
            var bank = await _servceProviderRepository.GetByIdAsync(bankId);
            if (bank == null)
            {
                throw new Exception($"Financial service provider not found.");
            }

            var contactPersons = await _contactPersonRepository.GetByAllByBankIdAsync(bankId, filter, status);
            return contactPersons.Select(x => new FinancialServiceProviderContactPersonResponseDto
            {
                FinancialServiceProviderId = x.FinancialServiceProviderId,
                Code = x.Code,
                Id = x.Id,
                FullName = x.FullName,
                Status = x.GeneralStatus.ToString()
            });
        }


        //public async Task MakeDepositAsync(CreateTransactionDto depositDto)
        //{
        //    var transaction = await _dbContext.Database.BeginTransactionAsync();

        //    // Validate bank exists
        //    var financialServiceProvider = await _servceProviderRepository.GetByIdAsync(depositDto.BusinessPartnerId.Value);
        //    if (financialServiceProvider == null)
        //    {
        //        throw new Exception($"Financial service provider not found.");
        //    }

        //    // Validate contact person belongs to the bank and is active
        //    var contactPerson = await _contactPersonRepository.GetByIdAsync(depositDto.BusinessPartnerId.Value);
        //    if (contactPerson == null || contactPerson.FinancialServiceProviderId != depositDto.BusinessPartnerId)
        //    {
        //        throw new Exception("Invalid contact person.");
        //    }

        //    if (contactPerson.GeneralStatus != GeneralStatus.Active)
        //    {
        //        throw new Exception("The selected contact person is not active.");
        //    }

        //    var currency = await _currencyRepository.GetByCurrencyCodeAsync(depositDto.CurrencyCode);

        //    if (currency == null) throw new Exception("Invalid currency");
        //    // Create deposit entity

        //    var user = await _userRepository.GetUserByRefreshTokenAsync();

        //    if (user == null) throw new Exception("User not found");
        //    // Create deposit entity
        //    try
        //    {
        //        await _transactionService.CompleteTransationProcess(depositDto, null );
        //    } catch(Exception ex)
        //    {
        //        await transaction.RollbackAsync();
        //        throw ex;
        //    }

           
        //    //}

        //    //return new DepositResponseDto
        //    //{
        //    //    FinancialServiceProviderId = financialServiceProvider.Id,
        //    //    FinancialServiceProviderName = financialServiceProvider.Name,
        //    //    Amount = depositDto.AmountPaid,
        //    //    ContactPersonId = contactPerson.Id,
        //    //    ContactPersonName = contactPerson.FullName
        //    //    //CreatedAt = tra.CreatedAt,
        //    //    //TransactionNumber = deposit.TransactionNumber,
        //    //    //DepositDate = deposit.DepositDate,
        //    //    //DepositId = deposit.Id,
        //    //    //Description = deposit.Description,
        //    //    //Status = deposit.Status.ToString()

        //    //};
        //}

        public async Task Disbursement(CreateFinancialServiceDisbursementDTO createDto)
        {
            var transaction = await _dbContext.Database.BeginTransactionAsync();
            var user = await _userRepository.GetUserByRefreshTokenAsync();
            if (user == null) throw new Exception("Unauthorized");

            var location = await _locationRepository.GetByIdAsync(createDto.LocationId);

            if (location == null) throw new Exception("Location not found");

            if (string.IsNullOrEmpty(createDto.CurrencyCode)) throw new Exception("Currency required exception");
            var currencyCode = await _dbContext.Currencies.FirstOrDefaultAsync(x => x.Code.ToUpper().Trim() == createDto.CurrencyCode.ToUpper().Trim());
            if (currencyCode == null) throw new Exception("Invalid currency");

            var validServiceProvider = await _servceProviderRepository.GetByIdAsync(createDto.FinancialServiceProviderId);
            if (validServiceProvider == null) throw new Exception("Financial service provider not found");

            var transactionNumber = await _transactionCodeRepository.GenerateTransactionCodeAsync("DEPO", location.Id);

            var transactionRecord = Transaction.Create(Guid.NewGuid(), transactionNumber, DateTime.SpecifyKind(createDto.Date ?? DateTime.UtcNow, DateTimeKind.Utc), createDto.TotalAmount, 0, 0, Guid.Parse(user.Id), DateTime.UtcNow, TransactionResultsType.FinancialServiceProviderDisbursement, TransactionType.DEPO.ToString(), location.Id, false);
           await  _dbContext.Transactions.AddAsync(transactionRecord);


             var paymentTransactionNumber = await _transactionCodeRepository.GenerateTransactionCodeAsync("TXP", transactionRecord.LocationId);

            var payment = Payment.Create(
                  Guid.NewGuid(),
                  transactionRecord.Id,
                  paymentTransactionNumber,
                   DateTime.SpecifyKind(createDto.Date ?? DateTime.UtcNow, DateTimeKind.Utc),
                  createDto.TotalAmount,
                  0,
                  createDto.PaymentMethod,
                  DateTime.UtcNow,
                  Guid.Parse(user.Id),
                  PaymentStatus.Completed ,
                  currencyCode.Id,
                  null
             );

            await _dbContext.TransactionPayments.AddAsync(payment);

            var disbursement = FinancialServiceDisbursement.Create(Guid.NewGuid(), transactionRecord.Id, validServiceProvider.Id);
            await _dbContext.FinancialServiceDisbursement.AddAsync(disbursement);

            var ContactPersonsToSave = new List<FinancialServiceProviderContactPerson>();
            var newContactPersonDisbursementList = new List<FinancialServiceDisbursementContactPerson>();
            var EmailReceiversList = new List<EmailReceiver>();

            foreach(var person in createDto.ContactPersons)
            {
                var exits = await _contactPersonRepository.GetByIdAsync(person.Id??Guid.Empty);

                if (exits == null)
                {
                    var ContactPerson = FinancialServiceProviderContactPerson.Create(Guid.NewGuid(), "", person.FullName, person.Email, person.PhoneNumber, validServiceProvider.Id, Guid.Parse(user.Id), DateTime.UtcNow, "");
                    ContactPersonsToSave.Add(ContactPerson);

                    exits = ContactPerson;
                }

                var contactPersonDisbursement = FinancialServiceDisbursementContactPerson.Create(Guid.NewGuid(), disbursement.Id, exits.Id, DateTime.UtcNow, Guid.Parse(user.Id));
                newContactPersonDisbursementList.Add(contactPersonDisbursement);

                var EmailReceiver = new EmailReceiver();
                EmailReceiver.Email = exits?.Email ?? "";
                EmailReceiver.Name = $"{exits?.FirstName ?? ""} {exits?.LastName ?? ""}";
                EmailReceiver.Code = exits?.Code ?? "";
                EmailReceiver.Id = exits?.Id ?? Guid.Empty;

                EmailReceiversList.Add(EmailReceiver);

            }

            if (ContactPersonsToSave.Count > 0)
            {
                await _dbContext.FinancialServiceProviderContactPersons.AddRangeAsync(ContactPersonsToSave);
            }

            await _dbContext.FinancialServiceDisbursementContactPersons.AddRangeAsync(newContactPersonDisbursementList);


            await _transactionService.SaveTransactionEmailTemplate(transactionRecord, user, payment, Guid.NewGuid(), EmailReceiversList);

            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
        }
    
        public async Task<IEnumerable<GetFinancialServiceDisbursementDTO>> GetDisbursements(Guid locationId, Guid? financialServiceProviderId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var disbursements = _dbContext.FinancialServiceDisbursement
                                    .Include(x => x.FinancialServiceProvider)
                                    .Include(x => x.FinancialServiceDisbursementContactPersons)
                                        .ThenInclude(x=> x.ContactPerson)
                                    .Include(x => x.Transaction)
                                        .ThenInclude(x => x.Location)
                                    .Where(x => x.Transaction.LocationId == locationId);

            if (financialServiceProviderId != null && financialServiceProviderId != Guid.Empty) {
                disbursements = disbursements.Where(x=> x.FinancialServiceProviderId == financialServiceProviderId);
            }

            if (startDate != null)
            {
                disbursements = disbursements.Where(x => x.Transaction.CreatedAt >=  startDate);
            }

            if (endDate != null)
            {
                disbursements = disbursements.Where(x => x.Transaction.CreatedAt <= endDate);
            }

            return await disbursements.Select(x => new GetFinancialServiceDisbursementDTO
            { 
                 CreatedAt = x.Transaction.CreatedAt,
                 LocationName = x.Transaction.Location.Name,
                 TotalAmount = x.Transaction.TotalAmount,
                 TransactionCode = x.Transaction.TransactionNumber,
                 FinacialServiceProviderName = x.FinancialServiceProvider.Name,
                 TransactionDate = x.Transaction.TransactionDate,
                 Id = x.Id,
                 ContactPersons = x.FinancialServiceDisbursementContactPersons.Select(cp => new ContactPersons
                 {
                     Id = cp.Id,
                     Email = cp.ContactPerson.Email,
                     FullName = cp.ContactPerson.FullName,
                     PhoneNumber = cp.ContactPerson.PhoneNumber
                     
                 }).ToList()

            }).ToListAsync();

        }

        public async Task<IEnumerable<FinancialServiceProviderResponseDto>> GetFinancialServiceProviders(Guid locationId, GeneralStatus? status)
        {
            var banks = _servceProviderRepository.GetFinancialServiceProvidersByLocationAsync(locationId, null, status ?? GeneralStatus.Active);
            return await banks.Select(x => new FinancialServiceProviderResponseDto
            {
               Code = x.Code,
               Address = x.Address,
               Id = x.Id,
               Name = x.Name, 
               Status = x.GeneralStatus.ToString(),
               ContactPersons = x.ContactPersons.Select(cp=> new FinancialServiceProviderContactPersonResponseDto
               {
                   Status = cp.GeneralStatus.ToString(),
                   Id = cp.Id,
                   Code = cp.Code,
                   FullName = cp.FullName,
               }).ToList(),
            }).ToListAsync();
        }
    }
}