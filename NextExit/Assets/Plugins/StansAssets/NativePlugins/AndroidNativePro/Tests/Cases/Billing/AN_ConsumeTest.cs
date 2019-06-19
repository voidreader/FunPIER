using UnityEngine;
using System.Collections;
using SA.Android.Vending.Billing;
using SA.Foundation.Templates;

using SA.Foundation.Tests;


namespace SA.Android.Tests.Billing
{
    public class AN_ConsumeTest : SA_BaseTest {


        public override bool RequireUserInteraction { get { return true; } }

        public override void Test() {

            AN_Billing.Consume(AN_Billing.Inventory.Purchases[0], (SA_Result result) => {
                SetAPIResult(result);
            });
        }
    }
}