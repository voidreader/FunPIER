using System;
using System.Collections.Generic;
using UnityEngine;

namespace SA.Android.Vending.Billing
{
    [Obsolete("Use AN_BillingClient API instead")]
    [Serializable]
    public class AN_Inventory
    {

        [SerializeField] List<AN_Product> m_products = new List<AN_Product>();
        [SerializeField] List<AN_Purchase> m_purchases = new List<AN_Purchase>();



        //--------------------------------------
        //  Products
        //--------------------------------------


        public List<AN_Product> Products {
            get {
                return m_products;
            }
        }


        public void SetProducts(List<AN_Product> products) {
            m_products = products;
        }



        /// <summary>
        /// Returns the <see cref="AN_Product"/> object registerd with the app.
        /// If project wasn't found result will be <c>null</c>
        /// </summary>
        /// <param name="productId">Id of the requested product</param>
        public AN_Product GetProductById(string productId) {
            foreach (var products in m_products) {
                if (products.ProductId.Equals(productId)) {
                    return products;
                }
            }

            return null;
        }



        /// <summary>
        /// <c>true</c> if product is registerd with the application
        /// otherwise returns <c>false</c>
        /// </summary>
        /// <param name="productId">Id of the requested product</param>
        public bool HasProductWithId(string productId) {
            return GetProductById(productId) != null;
        }


        //--------------------------------------
        //  Purchases
        //--------------------------------------

        /// <summary>
        /// List of the all purchases made by a user
        /// </summary>
        public List<AN_Purchase> Purchases {
            get {
                return m_purchases;
            }
        }


        public void SetPurchases(List<AN_Purchase> purchases) {
            m_purchases = purchases;
        }



        /// <summary>
        /// Returns the <see cref="AN_Purchase"/> object realted to the given product Id.
        /// If project wasn't found result will be <c>null</c>
        /// </summary>
        /// <param name="productId">Id of the requested product</param>
        public AN_Purchase GetPurchaseByProductId(string productId) {
            foreach (var purchase in m_purchases) {
                if (purchase.ProductId.Equals(productId)) {
                    return purchase;
                }
            }

            return null;
        }


        /// <summary>
        /// <c>true</c> if product with give id was purchased by a user
        /// otherwise returns <c>false</c>
        /// </summary>
        /// <param name="productId">Id of the requested product</param>
        public bool IsProdcutPurchased(string productId) {
            return GetPurchaseByProductId(productId) != null;
        }

    }
}