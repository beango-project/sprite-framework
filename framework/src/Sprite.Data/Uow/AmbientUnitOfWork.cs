using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using JetBrains.Annotations;

namespace Sprite.Data.Uow
{
    public class AmbientUnitOfWork
    {
        private static volatile AmbientUnitOfWork _current;

        private static AsyncLocal<IUnitOfWork> _unitOfWorkCell = new AsyncLocal<IUnitOfWork>(ValueChanged);


        public static AmbientUnitOfWork Current => _current;

        public IUnitOfWork? UnitOfWork
        {
            get
            {
                var current = _unitOfWorkCell.Value;
                while (current?.IsDisposed == true || current?.IsCompleted == true) //if current uow is disposed we change current uow to outer(previous) 
                {
                    _unitOfWorkCell.Value = current = current.Outer;
                }

                return current;
            }
        }

        private static void ValueChanged(AsyncLocalValueChangedArgs<IUnitOfWork> args)
        {
            if (args.ThreadContextChanged)
            {
                // if (!ExecutionContext.IsFlowSuppressed())
                // {
                //     ExecutionContext.SuppressFlow();
                // }

                var previous = args.PreviousValue;
                var current = args.CurrentValue;


                if (current == null || previous?.IsDisposed == false)
                {
                    _unitOfWorkCell.Value = previous;
                }

                // if ((current?.IsDisposed == true && ExecutionContext.IsFlowSuppressed()))
                // {
                //     ExecutionContext.RestoreFlow();
                // }
            }
        }

        public void SetUnitOfWork(IUnitOfWork unitOfWork)
        {
            _unitOfWorkCell.Value = unitOfWork;
            // if (_unitOfWorkCell.Value == null)
            // {
            //     _unitOfWorkCell.Value = new ScopedUnitOfWorkStorage();
            // }
            //
            // // var asyncLocalValueChangedArgs = 
            // // asyncLocalValueChangedArgs.PreviousValue
            // _unitOfWorkCell.Value.UnitOfWorks.Add(unitOfWork);
            // if (unitOfWork is UnitOfWork)
            // {
            //     _unitOfWorkCell.Value.ActualUnitOfWorks.Add(unitOfWork);
            //     unitOfWork.OnDisposed += (_, _) =>
            //     {
            //         _unitOfWorkCell.Value.ActualUnitOfWorks.Remove(unitOfWork);
            //     };
            // }
        }


        static AmbientUnitOfWork()
        {
            if (_current == null)
            {
                lock (typeof(AmbientUnitOfWork))
                {
                    if (_current == null)
                    {
                        _current = new AmbientUnitOfWork();
                    }
                }
            }
        }
    }
}