using System;
using System.Collections.Generic;

namespace Sprite.Web.Http.Models
{
    public class ControllerDefinition
    {
        // public ActionDefinition AddAction(string uniqueName, ActionDefinition action){
        //
        // }

        public ControllerDefinition()
        {
            Actions = new List<ActionDefinition>();
        }

        public ControllerDefinition(string controllerName, Type controllerType) : this()
        {
            ControllerName = controllerName;
            ControllerType = controllerType;
        }

        public string ControllerName { get; set; }

        public Type ControllerType { get; set; }
        public IList<ActionDefinition> Actions { get; set; }
    }
}