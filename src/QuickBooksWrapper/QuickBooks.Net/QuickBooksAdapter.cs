﻿using DevDefined.OAuth.Framework;
using Intuit.Ipp.Core;
using Intuit.Ipp.Data;
using Intuit.Ipp.DataService;
using Intuit.Ipp.Security;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QuickBooks.Net
{
    public class QuickBooksAdapter : IQuickBooksAdapter
    {
        public QuickBooksAdapter(IToken accessToken, String  appToken, String consumerKey, String consumerSecret, String baseUrl)
        {
            var oauthValidator = new OAuthRequestValidator(accessToken.Token, accessToken.TokenSecret, consumerKey, consumerSecret);

            Context = new ServiceContext(appToken, accessToken.Realm, IntuitServicesType.QBO, oauthValidator);
            Context.IppConfiguration.BaseUrl.Qbo = baseUrl;
        }

        public ServiceContext Context { get; set; }

        public IEnumerable<T> FindAll<T>() where T : IEntity, new()
        {
            var result = new List<T>();

            var customer = new T();
            var dataService = new DataService(Context);

            var position = 1;
            var isRunning = true;

            while (isRunning)
            {
                var entities = dataService.FindAll(customer, position).ToList();

                isRunning = entities.Any();
                if (isRunning)
                {
                    position += entities.Count;
                    result.AddRange(entities);
                }
            }

            return result;
        }

        public T Add<T>(T entity) where T : IEntity
        {
            return new DataService(Context).Add(entity);
        }

        public T Update<T>(T entity) where T : IEntity
        {
            return new DataService(Context).Update(entity);
        }

        public T Delete<T>(T entity) where T : IEntity
        {
            return new DataService(Context).Delete(entity);
        }
    }
}
