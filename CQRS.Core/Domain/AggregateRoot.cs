﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CQRS.Core.Events;

namespace CQRS.Core.Domain
{
    public abstract class AggregateRoot
    {
        protected Guid _id;
        private readonly List<BaseEvent> _changes = new();
        public Guid Id
        {
            get
            {
                return _id;
            }
            protected set
            {
                _id = value;
            }
        }
        public int Version { get; set; } = -1;

        public IEnumerable<BaseEvent> GetUncommitedChanges()
        {
            return _changes;
        }

        public void MarkChangesAsCommited()
        {
            _changes.Clear();
        }

        private void ApplyChanges(BaseEvent @event, bool isNew)
        {
            var method = this.GetType().GetMethod("Apply", new Type[] { @event.GetType() });

            if (method == null)
            {
                throw new ArgumentException(nameof(method), $"The Apply method was not found in the aggregate for {@event.GetType().Name}");
            }

            method.Invoke(this, new object[] { @event });

            if (isNew)
            {
                _changes.Add(@event);
            }
        }

        protected void RaiseEvent(BaseEvent @event)
        {
            ApplyChanges(@event, true);
        }

        protected void ReplayEvent(IEnumerable<BaseEvent> @events)
        {
            foreach (var @event in @events)
            {
                ApplyChanges(@event, false);
            }
        }
    }
}
