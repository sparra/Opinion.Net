
namespace Opinion.Infrastructure.Common.Utility
{
    using AutoMapper;
    using System;
    public class Mappers
    {
        //private readonly IMappingEngineFull full;
        //private readonly IMappingEnginePartial partial;
        private readonly MappingEngine full;
        private readonly MappingEngine partial;
        
        public Mappers(MappingEngine full, MappingEngine partial)
        {            
            this.full = full;
            this.partial = partial;
        }
        public MappingEngine MapperFull
        {
            get
            {                
                return full;
            }
        }
        public MappingEngine MapperPartial
        {
            get
            {
                return partial;
            }
        }

        //public void configurefull(Action<IConfiguration> action)
        //{
        //    this.MapperFull.Reset();

        //    action(MapperFull..Configuration);

        //                Configuration.Seal();
        //}
    }
}
