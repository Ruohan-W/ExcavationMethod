using Autodesk.Revit.DB;
using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcavationMethod.Revit.Utilities.Messages
{
    public class ChangedElementIdWrapper
    {
        public Document? Document { get; set; }
        public ICollection<ElementId>? AddedElementIds { get; set; }
        public ICollection<ElementId>? DeletedElementIds { get; set; }
        public ICollection<ElementId>? ModifiedElementIds { get; set; }
    }

    public class ChangedElementMessage : ValueChangedMessage<ChangedElementIdWrapper>
    {
        public ChangedElementMessage(ChangedElementIdWrapper value) : base(value)
        {

        }
    }
}
