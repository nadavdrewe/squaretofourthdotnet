using System;
using System.Collections.Generic;
using System.Text;

namespace com.fourth.pipeline.pos
{
    public static class TransactionTypeCodes
    {
        public static string TAB_OPEN = "TAB_OPEN";
        public static string TAB_CLOSE = "TAB_CLOSE";

        public static string MODIFIER_ITEM = "MODIFIER_ITEM";
        public static string SALES_ITEM = "SALES_ITEM";
        public static string TENDER = "TENDER";
        public static string DISC_ITEM = "DISC_ITEM"; //remember can be 2 types of discount - order and item!
        public static string SERVICE_CHARGE = "SERVICE_CHARGE"; //doesnt't exist in square - double check
        public static string MAINS_AWAY = "MAINS_AWAY"; //leave for now
        public static string PRINT_CHECK = "PRINT_CHECK"; //nothing in this, just ignore for now

        //voided items
        public static string VOID_ERROR = "VOID_ERROR"; 
        public static string VOID_ITEM = "VOID_ITEM"; 
        public static string VOID_WASTE = "VOID_WASTE";

  
    }
}

