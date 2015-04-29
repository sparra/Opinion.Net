using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorkerRoleCommandProcessor
{
    using System.Data.Entity;
    using Opinion.Infrastructure.CQRS;
    using Opinion.Infrastructure.CQRS.BlobStorage;
    using Opinion.Infrastructure.CQRS.EventSourcing;
    using Opinion.Infrastructure.CQRS.Messaging;
    using Opinion.Infrastructure.CQRS.Messaging.Handling;
    using Opinion.Infrastructure.CQRS.Serialization;
    using Opinion.Infrastructure.Sql.BlobStorage;
    using Opinion.Infrastructure.Sql.EventSourcing;
    using Opinion.Infrastructure.Sql.MessageLog;
    using Opinion.Infrastructure.Sql.Messaging;
    using Opinion.Infrastructure.Sql.Messaging.Handling;
    using Opinion.Infrastructure.Sql.Messaging.Implementation;
    using Autofac;
    using Autofac.Core;

    // <summary>
    /// Local-side of the processor, which is included for compilation conditionally 
    /// at the csproj level.
    /// </summary>
    /// <devdoc>
    /// NOTE: this file is only compiled on DebugLocal configurations. In non-DebugLocal 
    /// you will not see full syntax coloring, intellisense, etc.. But it is still 
    /// much more readable and usable than a grayed-out piece of code inside an #if
    /// </devdoc>
    partial class OpinionProcessor
    {
        partial void OnCreateContainer(IContainer container)
        {
            var serializer = container.Resolve<ITextSerializer>();
            var metadata = container.Resolve<IMetadataProvider>();

            var newBuilder = new ContainerBuilder();
            newBuilder.RegisterType<SqlBlobStorage>().As<IBlobStorage>().WithParameter(new NamedParameter("nameOrConnectionString", "BlobStorage"));

            var commandBus = new CommandBus(new MessageSender(Database.DefaultConnectionFactory, "SqlBus", "SqlBus.Commands"), serializer);
            var eventBus = new EventBus(new MessageSender(Database.DefaultConnectionFactory, "SqlBus", "SqlBus.Events"), serializer);

            var commandProcessor = new CommandProcessor(new MessageReceiver(Database.DefaultConnectionFactory, "SqlBus", "SqlBus.Commands"), serializer);
            var eventProcessor = new EventProcessor(new MessageReceiver(Database.DefaultConnectionFactory, "SqlBus", "SqlBus.Events"), serializer);

            newBuilder.RegisterInstance(commandBus).As<ICommandBus>();
            newBuilder.RegisterInstance(eventBus).As<IEventBus>();
            newBuilder.RegisterInstance(commandProcessor).As<ICommandHandlerRegistry>();
            //newBuilder.RegisterInstance(commandProcessor).Named<IProcessor>("CommandProcessor");
            newBuilder.RegisterInstance(commandProcessor).As<IProcessor>().WithMetadata("Name", "CommandProcessor");
            newBuilder.RegisterInstance(eventProcessor).As<IEventHandlerRegistry>();
            //newBuilder.RegisterInstance(eventProcessor).Named<IProcessor>("EventProcessor");
            newBuilder.RegisterInstance(eventProcessor).As<IProcessor>().WithMetadata("Name","EventProcessor");

            // Event log database and handler.
            newBuilder.RegisterType<SqlMessageLog>().
                WithParameters(new Parameter[]
               {
                new NamedParameter("nameOrConnectionString", "MessageLog"),
                new NamedParameter("serializer", serializer),
                new NamedParameter("metadata", metadata)
               });

            newBuilder.RegisterType<SqlMessageLogHandler>().Named<IEventHandler>("SqlMessageLogHandler").AsSelf();
            //newBuilder.RegisterType<global::Opinion.Domain.Programa.CatProgramaEventHandler>().As<IEventHandler>().AsSelf();

            newBuilder.RegisterType<SqlMessageLogHandler>().Named<ICommandHandler>("SqlMessageLogHandler").AsSelf();
            
            newBuilder.Update(container);

            RegisterRepository(container);
            RegisterEventHandlers(container, eventProcessor);
            RegisterCommandHandlers(container);
        }

        private void RegisterEventHandlers(IContainer container, EventProcessor eventProcessor)
        {
            /*eventProcessor.Register(container.Resolve<RegistrationProcessManagerRouter>());
            eventProcessor.Register(container.Resolve<DraftOrderViewModelGenerator>());
            eventProcessor.Register(container.Resolve<PricedOrderViewModelGenerator>());
            eventProcessor.Register(container.Resolve<ConferenceViewModelGenerator>());
            eventProcessor.Register(container.Resolve<SeatAssignmentsViewModelGenerator>());
            eventProcessor.Register(container.Resolve<SeatAssignmentsHandler>());
            eventProcessor.Register(container.Resolve<global::Conference.OrderEventHandler>());*/
            //eventProcessor.Register(container.Resolve<global::Opinion.Domain.Programa.CatProgramaEventHandler>());
            eventProcessor.Register(container.Resolve<SqlMessageLogHandler>());
        }

        private void RegisterRepository(IContainer container)
        {
            var newBuilder = new ContainerBuilder();
            // repository
            //Transient
            newBuilder.RegisterType<EventStoreDbContext>().WithParameter(new NamedParameter("nameOrConnectionString", "EventStore")).AsSelf().InstancePerDependency();
            newBuilder.RegisterGeneric(typeof(SqlEventSourcedRepository<>)).As(typeof(IEventSourcedRepository<>));
            newBuilder.Update(container);
        }

        private static void RegisterCommandHandlers(IContainer container)
        {
            var commandHandlerRegistry = container.Resolve<ICommandHandlerRegistry>();

            foreach (var commandHandler in container.Resolve<IEnumerable<ICommandHandler>>())
            {
                commandHandlerRegistry.Register(commandHandler);
            }
        }
           
    }
}
