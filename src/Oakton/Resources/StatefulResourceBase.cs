﻿using System.Threading;
using System.Threading.Tasks;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Oakton.Resources
{
    /// <summary>
    /// Base class with empty implementations for IStatefulResource.
    /// </summary>
    public abstract class StatefulResourceBase : IStatefulResource
    {
        protected StatefulResourceBase(string type, string name)
        {
            Type = type;
            Name = name;
        }

        public virtual Task Check(CancellationToken token) => Task.CompletedTask;

        public virtual Task ClearState(CancellationToken token) => Task.CompletedTask;

        public virtual Task Teardown(CancellationToken token) => Task.CompletedTask;

        public virtual Task Setup(CancellationToken token) => Task.CompletedTask;

        public virtual Task<IRenderable> DetermineStatus(CancellationToken token)
        {
            return Task.FromResult((IRenderable)new Markup("Okay"));
        }

        public string Type { get; }
        public string Name { get; }
    }
}