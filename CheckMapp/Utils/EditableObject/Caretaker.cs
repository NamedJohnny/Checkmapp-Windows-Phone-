using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckMapp.Utils.EditableObject
{
    public class Caretaker<T> : IEditableObject
    {
        private Memento<T> memento;
        private T target;

        public T Target
        {
            get
            {
                return this.target;
            }
            protected set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Target", "Target cannot be null");
                }

                if (Object.ReferenceEquals(this.Target, value))
                    return;

                this.target = value;
            }
        }

        public Caretaker(T target)
        {
            this.Target = target;
        }

        public void BeginEdit()
        {
            if (this.memento == null)
                this.memento = new Memento<T>(this.Target);
        }

        public void CancelEdit()
        {
            if (this.memento != null)
            {

                this.memento.Restore(Target);
                this.memento = null;
            }
        }

        public void EndEdit()
        {
            if (this.memento != null)
            {
                this.memento = null;
            }
        }
    }
}
