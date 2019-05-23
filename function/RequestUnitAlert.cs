

namespace Microsoft.CosmosDb.Autoscale 
{
    public class RequestUnitAlert
    {
        public string Name { get; }
        public double Value { get; set; }

        public string AccountName { get; set; }
        public string DatabaseName { get; set; }

        public RequestUnitAlert(string name)
        {
            Name = name;
        }
  }
}