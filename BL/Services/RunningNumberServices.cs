using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using DataModel;
using DataModel.UnitOfWork;

namespace BL
{
    public class RunningNumberServices : IRunningNumberServices
    {
        private readonly UnitOfWork _unitOfWork;
        public RunningNumberServices(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public int GetNumber(string key)
        {
            int number = 1;
            using (var scope = new TransactionScope())
            {
                var running = _unitOfWork.RunningNumberRepository.GetFirstOrDefault(x => x.RunningKey.ToUpper() == key.ToUpper());
                if (running != null)
                {
                    number = running.RunningNo.GetValueOrDefault() + 1;

                    RunningNumber rn = new RunningNumber();
                    rn.RunningKey = key.ToUpper();
                    rn.RunningNo = number;

                    _unitOfWork.RunningNumberRepository.Update(rn);
                }
                else
                {
                    RunningNumber rn = new RunningNumber();
                    rn.RunningKey = key.ToUpper();
                    rn.RunningNo = number;

                    _unitOfWork.RunningNumberRepository.Insert(rn);
                }
                _unitOfWork.Save();
                _unitOfWork.RunningNumberRepository.DetachAll();
                scope.Complete();
            }
            return number;
        }
    }
}
