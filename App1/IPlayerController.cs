using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1
{
    internal interface IPlayerController
    {
        void NextTarget(TargetPlayerStatus targetStatus);
        event EventHandler<CurrentPlayerStatus> CurrentStatusChanged;
    }
}
