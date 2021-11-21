using System;

namespace Sprite.Transactions.Sagas.Commands
{
    public class StateCommand : Command
    {
        /// <summary>
        /// 状态
        /// </summary>
        public string StateName { get; }

        /// <summary>
        /// 状态机名称
        /// </summary>
        public string StateMachineName { get; }

        public IState TemporaryState { get; }

        public IState State(IProcessContext context)
        {
            if (TemporaryState != null)
            {
                return TemporaryState;
            }

            if (string.IsNullOrEmpty(StateMachineName))
            {
                throw new Exception();
            }

            return null;
        }
    }
}