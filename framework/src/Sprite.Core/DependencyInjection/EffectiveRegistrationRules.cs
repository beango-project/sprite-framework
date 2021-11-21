using System.Collections.Generic;

namespace Sprite.DependencyInjection
{
    public class EffectiveRegistrationRules
    {
        public EffectiveRegistrationRules()
        {
            RulesCollection = new LinkedList<IRegistrationRules>();
        }

        public LinkedList<IRegistrationRules> RulesCollection { get; }


        public void AddRules(IRegistrationRules rules)
        {
            RulesCollection.AddIfNotContains(rules);
        }

        // TODO: 可以设置跳过某个注册规则？
        public void SkipRules(IRegistrationRules rules)
        {
        }

        public void RemoveRules(IRegistrationRules rules)
        {
            RulesCollection.Remove(rules);
        }
    }
}