using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Objects;
using System;
using Xunit;

namespace ToDoApp.Components.Mvc
{
    public class TransactionFilterTests
    {
        [Fact]
        public void OnResourceExecuting_DoesNothing()
        {
            using DbContext testingContext = TestingContext.Create();
            ActionContext action = new(new DefaultHttpContext(), new RouteData(), new ActionDescriptor());
            ResourceExecutingContext context = new(action, Array.Empty<IFilterMetadata>(), Array.Empty<IValueProviderFactory>());

            new TransactionFilter(testingContext).OnResourceExecuting(context);
        }

        [Fact]
        public void OnResourceExecuted_Exception_Rollbacks()
        {
            ActionContext action = new(new DefaultHttpContext(), new RouteData(), new ActionDescriptor());
            ResourceExecutedContext context = new(action, Array.Empty<IFilterMetadata>());
            using DbContext currentContext = TestingContext.Create();
            using DbContext testingContext = TestingContext.Create();
            Role role = ObjectsFactory.CreateRole(0);
            context.Exception = new Exception();

            testingContext.Drop();

            TransactionFilter filter = new(testingContext);

            testingContext.Add(role);
            testingContext.SaveChanges();

            Assert.Empty(currentContext.Set<Role>());
            Assert.Single(testingContext.Set<Role>());

            filter.OnResourceExecuted(context);

            Assert.Empty(currentContext.Set<Role>());
            Assert.Empty(testingContext.Set<Role>());
        }

        [Fact]
        public void OnResourceExecuted_CommitsTransaction()
        {
            ActionContext action = new(new DefaultHttpContext(), new RouteData(), new ActionDescriptor());
            ResourceExecutedContext context = new(action, Array.Empty<IFilterMetadata>());
            using DbContext currentContext = TestingContext.Create();
            using DbContext testingContext = TestingContext.Create();
            Role role = ObjectsFactory.CreateRole(0);

            testingContext.Drop();

            TransactionFilter filter = new(testingContext);

            testingContext.Add(role);
            testingContext.SaveChanges();

            Assert.Empty(currentContext.Set<Role>());
            Assert.Single(testingContext.Set<Role>());

            filter.OnResourceExecuted(context);

            Assert.Single(currentContext.Set<Role>());
            Assert.Single(testingContext.Set<Role>());
        }
    }
}
