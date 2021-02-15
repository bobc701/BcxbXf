using BCX.BCXCommon;
using BcxbDataAccess;
using System;
using System.Collections.ObjectModel;

namespace BcxbXf.Extend
{
    public class LeftComponent
    {
        public object Name { get; set; }

        public ObservableCollection<CTeamRecord> RightComponentList { get; set; }
    }



    public class RightComponent
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public override string ToString()
        {
            return string.Format("[RightComponent: Id={0}, Name={1}]", Id, Name);
        }
    }
}
