using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;

namespace Tablazor.Components
{
    public class TablerComponent : ComponentBase
    {
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        private HashSet<string> ClassSet { get; set; } = new();

        public string Class => string.Join(" ", ClassSet);

        private bool _active;
        [Parameter]
        public bool Active
        {
            get => _active;
            set
            {
                if (value)
                {
                    AddClass("active");
                }
                else
                {
                    RemoveClass("active");
                }
                _active = value;
            }
        }

        private bool _disabled;
        [Parameter]
        public bool Disabled
        {
            get => _disabled;
            set
            {
                if (value)
                {
                    AddClass("disabled");
                }
                else
                {
                    RemoveClass("disabled");
                }
                _disabled = value;
            }
        }
        
        protected override void OnInitialized()
        {
            base.OnInitialized();
            Initialize();
        }

        public virtual void Initialize()
        {
            
        }
        
        /// <summary>
        /// Adds a single class to the component if it's not already present.
        /// </summary>
        /// <param name="className">The class name to add.</param>
        
        public void AddClass(string className)
        {
            ClassSet.Add(className);
        }

        /// <summary>
        /// Adds multiple classes to the component if they're not already present.
        /// Classes should be provided as a space-separated string.
        /// </summary>
        /// <param name="classNames">The space-separated class names to add.</param>
        public void AddClasses(string classNames)
        {
            foreach (var className in classNames.Split(' '))
            {
                ClassSet.Add(className);
            }
        }

        /// <summary>
        /// Removes a single class from the component.
        /// </summary>
        /// <param name="className">The class name to remove.</param>
        public void RemoveClass(string className)
        {
            ClassSet.Remove(className);
        }

        /// <summary>
        /// Removes multiple classes from the component.
        /// Classes should be provided as a space-separated string.
        /// </summary>
        /// <param name="classNames">The space-separated class names to remove.</param>
        public void RemoveClasses(string classNames)
        {
            foreach (var className in classNames.Split(' '))
            {
                ClassSet.Remove(className);
            }
        }
    }
}
