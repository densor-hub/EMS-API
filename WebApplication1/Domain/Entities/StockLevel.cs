namespace WebApplication1.Domain.Entities
{
    public class StockLevel : BaseEntity
    {
        public int ActualQuantity { get; private set; } //UOM_QUANTITY
        public int AvailableQuanity { get; private set; } //UOM_QUANTITY
        public Guid ItemId { get; private set; }
        public  Item Item { get; private set; }
        public Guid LocationId { get; private set; }
        public  Location Location { get; private set; }

        private StockLevel()
        {
            
        }

        private StockLevel(Guid id, int actualQuantity, int availableQuantity, Guid itemId, Guid locationId)
        {
            Id= id;
            ActualQuantity= actualQuantity;
            AvailableQuanity= availableQuantity;
            ItemId= itemId;
            LocationId= locationId;
        }

        public static StockLevel Create(Guid id, int actualQuantity, int availableQuantity, Guid itemId, Guid locationId)
       => new StockLevel(id, actualQuantity, availableQuantity, itemId, locationId);


        public void AddActual(int increament)
        {
            ActualQuantity+= increament;
        }

       

        public void AddAvailable(int increament)
        {
            AvailableQuanity += increament;
        }

        public void SubstractActual(int decreament)
        {
            if (ActualQuantity - decreament < 0) throw new Exception($"Transaction will result in negative stock value. Actual Quantity = {AvailableQuanity}");
            ActualQuantity -= decreament;
        }

        public void SubstractAvailable(int decreament)
        {
            if (AvailableQuanity - decreament < 0) throw new Exception($"Transaction will result in negative stock value. Available Quantity = {AvailableQuanity}");
            AvailableQuanity -= decreament;
        }
    }
}
