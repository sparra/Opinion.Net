
using System.Collections;

namespace WorkerRoleCommandProcessor
{
    using Autofac;
    using Opinion.Infrastructure.CQRS;
    using Opinion.Infrastructure.CQRS.Serialization;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;

    public sealed partial class OpinionProcessor : IDisposable
    {
        private IContainer container;        
        private CancellationTokenSource cancellationTokenSource;
        private List<IProcessor> processors;
        private bool instrumentationEnabled;

        public OpinionProcessor(bool instrumentationEnabled = false)
        {
            this.instrumentationEnabled = instrumentationEnabled;

            OnCreating();

            this.cancellationTokenSource = new CancellationTokenSource();
            this.container = CreateContainer();

            this.processors = this.container.Resolve<IEnumerable<IProcessor>>().ToList();
        }

        public void Start()
        {
            this.processors.ForEach(p => p.Start());
        }

        public void Stop()
        {
            this.cancellationTokenSource.Cancel();

            this.processors.ForEach(p => p.Stop());
        }

        private IContainer CreateContainer()
        {
            var builder = new ContainerBuilder();

            // infrastructure

            builder.RegisterInstance<ITextSerializer>(new JsonTextSerializer());
            builder.RegisterInstance<IMetadataProvider>(new StandardMetadataProvider());


            // Integration (transient)
            //builder.RegisterType<global::Opinion.Domain.Persona.PersonaContext>().InstancePerDependency();
            //builder.RegisterType<global::Opinion.Domain.Programa.ProgramaContext>().InstancePerDependency();
            //builder.Register<Func<global::Opinion.Domain.Programa.ProgramaContext>>(                
            //    ctx => {
            //                var cc = ctx.Resolve<IComponentContext>();
            //                return () => cc.Resolve<global::Opinion.Domain.Programa.ProgramaContext>();                            
            //            }
            //);

            // handlers

            container = builder.Build();
            
            OnCreateContainer(container);
            return container;
        }

        partial void OnCreating();
        partial void OnCreateContainer(IContainer container);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private void Dispose(bool disposing)
        {
            if (disposing)
                if (cancellationTokenSource != null)
                {
                    cancellationTokenSource.Dispose();
                    cancellationTokenSource = null;
                }
        }
        ~OpinionProcessor()
        {
            Dispose(false);
        }
    }
}
