using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace workflow_core_practice
{
    class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddWorkflow();

            services.AddTransient<GoogdBye>();

            var serviceProvider = services.BuildServiceProvider();
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            loggerFactory.CreateLogger("Debug");

            var host = serviceProvider.GetService<IWorkflowHost>();
            host.RegisterWorkflow<HelloWorkflow>();
            host.Start();

            host.StartWorkflow("Hello, World");

            Console.ReadLine();
            host.Stop();
        }
    }

    class HelloWorkd : StepBody
    {
        public override ExecutionResult Run(IStepExecutionContext context)
        {
            Console.WriteLine("Hello, World");
            return ExecutionResult.Next();
        }
    }

    class GoogdBye : StepBody
    {
        private readonly ILogger _logger;

        public GoogdBye(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<GoogdBye>();
        }

        public override ExecutionResult Run(IStepExecutionContext context)
        {
            Console.WriteLine("Good Bye");
            _logger.LogInformation("Hi, there");
            return ExecutionResult.Next();
        }
    }

    class HelloWorkflow : IWorkflow
    {
        public void Build(IWorkflowBuilder<object> builder)
        {
            builder.StartWith<HelloWorkd>().Then<GoogdBye>();
        }

        public string Id => "Hello, World";
        public int Version => 1;
    }
}