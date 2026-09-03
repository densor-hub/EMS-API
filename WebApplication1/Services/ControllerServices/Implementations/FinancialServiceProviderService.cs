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

namespace WebApplication1.Services.ControllerServices.Implementations
{
    public class FinancialServiceProviderService : IFinancialServiceProviderService
    {
        private readonly IFinancialServiceProviderRepository _bankRepository;
        private readonly IFinancialServiceProviderContactPersonRepository _contactPersonRepository;
        private readonly ITransactionCodeRepository _transactionCodeRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICurrencyRepository _currencyRepository;
        private readonly ITransactionService _transactionService;

        //for transaction
        private readonly AppDbContext _db;
        public FinancialServiceProviderService(
             IFinancialServiceProviderRepository bankRepository,
             IFinancialServiceProviderContactPersonRepository contactPersonRepository,
             ITransactionCodeRepository transactionCodeRepository,
             ILocationRepository locationRepository,
             IUserRepository userRepository,
              ICurrencyRepository currencyRepository,
             AppDbContext db
            )
        {
            _bankRepository = bankRepository;
            _contactPersonRepository = contactPersonRepository;
            _transactionCodeRepository = transactionCodeRepository;
            _locationRepository = locationRepository;
            _userRepository = userRepository;
            _currencyRepository = currencyRepository;
            _db = db;

        }

        public async Task<FinancialServiceProviderResponseDto> CreateBankAsync(CreateFinancialServiceProviderDto createDto)
        {
            // Validate unique code
            var validLocation = await _locationRepository.GetByIdAsync(createDto.LocationId);
            if (validLocation == null) { throw new Exception("Invalid shop submitted"); }

            var validType = Enum.IsDefined(typeof(FinancialServiceProviderType), createDto.Type);
            if (validType == false) throw new Exception("Invalid type detected");

            var user = await _userRepository.GetUserByRefreshTokenAsync();

            if(user == null)
            {
                throw new Exception("Invalid User");
            }
            var transaction = await _db.Database.BeginTransactionAsync();

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

                await _bankRepository.AddAsync(bank);

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
                            DateTime.UtcNow
                        );

                        contactPersons.Add(contactPerson);
                    }
                }

                if (contactPersons.Any()) await _contactPersonRepository.AddRangeAsync(contactPersons);

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

            var bank = await _bankRepository.GetByIdAsync(bankId);
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

            var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                // Update bank basic info
                bank.Update(updateDto.Name ?? bank.Name, updateDto.Type ?? bank.Type, updateDto.Address, (GeneralStatus)updateDto.Status, Guid.Parse(user.Id));


                await _bankRepository.Update(bank);

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
                               Guid.Parse(user.Id)
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
                                DateTime.UtcNow
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
            var bank = await _bankRepository.GetByIdAsync(bankId);
            if (bank == null)
            {
                throw new Exception($"Financial service provider not found.");
            }

            // Soft delete bank
            bank.Update(bank.Name,bank.Type, bank.Address, GeneralStatus.SoftDeleted, Guid.Parse(user.Id));
            await _bankRepository.Update(bank);

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
            var banks =  _bankRepository.GetFinancialServiceProvidersByLocationAsync(locationId, null, status ?? GeneralStatus.Active);
            return  await banks.Select(x => new FinancialServiceProviderDropdownDto
            {
                Code = x.Code,
                Id = x.Id,
                Name = x.Name
            }).ToListAsync();
        }

        public async Task<IEnumerable<FinancialServiceProviderContactPersonResponseDto>> GetAllContactPersonsAsync(Guid bankId, string? filter, GeneralStatus? status)
        {
            var bank = await _bankRepository.GetByIdAsync(bankId);
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


        public async Task MakeDepositAsync(CreateTransactionDto depositDto)
        {
            // Validate bank exists
            var financialServiceProvider = await _bankRepository.GetByIdAsync(depositDto.BusinessPartnerId.Value);
            if (financialServiceProvider == null)
            {
                throw new Exception($"Financial service provider not found.");
            }

            // Validate contact person belongs to the bank and is active
            var contactPerson = await _contactPersonRepository.GetByIdAsync(depositDto.BusinessPartnerId.Value);
            if (contactPerson == null || contactPerson.FinancialServiceProviderId != depositDto.BusinessPartnerId)
            {
                throw new Exception("Invalid contact person.");
            }

            if (contactPerson.GeneralStatus != GeneralStatus.Active)
            {
                throw new Exception("The selected contact person is not active.");
            }

            var currency = await _currencyRepository.GetByCurrencyCodeAsync(depositDto.CurrencyCode);

            if (currency == null) throw new Exception("Invalid currency");
            // Create deposit entity

            var user = await _userRepository.GetUserByRefreshTokenAsync();

            if (user == null) throw new Exception("User not found");
            // Create deposit entity
            var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                await _transactionService.CompleteTransationProcess(depositDto, null );
            } catch(Exception ex)
            {
                await transaction.RollbackAsync();
                throw ex;
            }

           
            //}

            //return new DepositResponseDto
            //{
            //    FinancialServiceProviderId = financialServiceProvider.Id,
            //    FinancialServiceProviderName = financialServiceProvider.Name,
            //    Amount = depositDto.AmountPaid,
            //    ContactPersonId = contactPerson.Id,
            //    ContactPersonName = contactPerson.FullName
            //    //CreatedAt = tra.CreatedAt,
            //    //TransactionNumber = deposit.TransactionNumber,
            //    //DepositDate = deposit.DepositDate,
            //    //DepositId = deposit.Id,
            //    //Description = deposit.Description,
            //    //Status = deposit.Status.ToString()

            //};
        }


    }
}