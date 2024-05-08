namespace TechLanches.Producao.Adapter.RabbitMq
{
    public class PedidoMessage
    {
        public int Id { get; private set; }
        public string Cpf { get; private set; }

        public PedidoMessage(int id, string cpf)
        {
            Id = id;
            Cpf = cpf;
        }

    }
}
