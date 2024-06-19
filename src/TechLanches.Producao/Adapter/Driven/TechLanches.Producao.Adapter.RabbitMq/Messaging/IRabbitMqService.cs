namespace TechLanches.Producao.Adapter.RabbitMq.Messaging
{
    public interface IRabbitMqService
    {
        void Publicar(PedidoStatusMessage data);
        Task Consumir(Func<PedidoMessage, Task> function);
    }
}
