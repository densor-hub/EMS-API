namespace WebApplication1.Domain.Entities
{
    public class TransDisbursementExternalResponse
    {
        public Guid Id { get; set; }
        public Guid TransactionId { get; private set; }
        public Transaction Transaction { get; set; }
        public string PersonCode {get; private set; }
        public bool ResponseValue { get; private set; }
        public DateTime RespondedAt { get; private set; }
        public string IpAddress { get; private set; }
        public string Remarks { get; private set; }
        


        private TransDisbursementExternalResponse()
        {
            
        }


        private TransDisbursementExternalResponse(Guid id, Guid transactionId, string personCode, bool responseValue, DateTime respondedAt, string ipdAddress, string remarks)
        {
            Id = id;
            PersonCode = personCode;
            ResponseValue = responseValue;
            RespondedAt = respondedAt;
            IpAddress = ipdAddress;
            Remarks = remarks;
            TransactionId = transactionId;

        }

        public static TransDisbursementExternalResponse Create(Guid id, Guid transactionId, string personCode, bool responseValue, DateTime respondedAt, string ipdAddress, string remarks)
        => new TransDisbursementExternalResponse(id, transactionId, personCode,   responseValue, respondedAt, ipdAddress, remarks);
    }
}
