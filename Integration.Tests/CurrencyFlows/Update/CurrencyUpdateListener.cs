using IdelPog.Messaging.Listeners;
using IdelPog.SimulationEngine.Currency.DTO;

namespace Integration.Tests.CurrencyFlows.Update
{
    internal class CurrencyUpdateListener : IBufferListener<CurrencyUpdateDTO>
    {
        public Type ListenerType { get; } = typeof(CurrencyUpdateDTO);
        public IReadOnlyList<CurrencyUpdateDTO>? Buffer { get; private set; }
        public bool WasCalled { get; private set; }
        
        public void Handle(IReadOnlyList<CurrencyUpdateDTO> buffer)
        {
            WasCalled = true;
            Buffer = buffer;
            
            foreach (CurrencyUpdateDTO currencyUpdateDTO in buffer)
            {
                Console.WriteLine($"Amount : {currencyUpdateDTO.Amount}");
                Console.WriteLine($"CurrencyType : {currencyUpdateDTO.Currency}");
                Console.WriteLine($"Action : {currencyUpdateDTO.Action}");
            }
        }
    }
}