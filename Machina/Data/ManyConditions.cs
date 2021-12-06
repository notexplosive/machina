using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Data
{
    public class ManyConditions
    {
        protected readonly List<Func<bool>> conditions = new List<Func<bool>>();

        public void AddCondition(Func<bool> condition)
        {
            this.conditions.Add(condition);
        }

        public bool AllTrue(bool defaultAnswer = true)
        {
            if (this.conditions.Count == 0)
            {
                return defaultAnswer;
            }

            foreach (var condition in conditions)
            {
                if (condition.Invoke() == false)
                {
                    return false;
                }
            }

            return true;
        }

        public bool AnyTrue(bool defaultAnswer = true)
        {
            if (this.conditions.Count == 0)
            {
                return defaultAnswer;
            }

            foreach (var condition in conditions)
            {
                if (condition.Invoke() == true)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
