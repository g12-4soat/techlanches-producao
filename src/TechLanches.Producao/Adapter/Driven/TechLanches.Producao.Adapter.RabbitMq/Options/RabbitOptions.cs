﻿namespace TechLanches.Producao.Adapter.RabbitMq.Options
{
    public class RabbitOptions
    {
        public string Host { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string Queue { get; set; }
        public string QueueOrderStatus { get; set; }
        public uint PrefetchSize { get; set; }
        public ushort PrefetchCount { get; set; }
    }
}
