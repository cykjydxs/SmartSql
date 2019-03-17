﻿using SmartSql.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Configuration.Tags
{
    public class IsGreaterThan : NumericalCompareTag
    {
        public override bool IsCondition(RequestContext context)
        {
            Object reqVal = GetPropertyValue(context);
            if (reqVal == null) { return false; }

            Decimal reqValNum = 0M;
            if (reqVal is Enum)
            {
                reqValNum = Convert.ToDecimal(reqVal);
            }
            else
            {
                if (!Decimal.TryParse(reqVal.ToString(), out reqValNum)) { return false; }
            }
            return reqValNum > CompareValue;
        }
    }
}
