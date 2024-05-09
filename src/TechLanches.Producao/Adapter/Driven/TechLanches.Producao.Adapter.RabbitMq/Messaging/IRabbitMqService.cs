namespace TechLanches.Producao.Adapter.RabbitMq.Messaging
{
    public interface IRabbitMqService
    {
        void Publicar(int data);
        Task Consumir(Func<PedidoMessage, Task> function);
    }
}
