using System.Collections.Generic;

namespace IMCommon.TransferModels
{
    public class ContactAddServerResponseModelList
    {
        public List<ContactAddServerResponseModel> List;
        public ContactAddServerResponseModelList(){}

        public ContactAddServerResponseModelList(List<ContactAddServerResponseModel> list)
        {
            List = list;
        }
    }
}