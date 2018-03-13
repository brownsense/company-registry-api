using Moq;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;

namespace marketplace.api.test
{
    public static class TestHelper
    {
        public static DbSet<T> ToDbSet<T>(this IEnumerable<T> data) where T : class
        {
            var fakeTransactions = data.AsQueryable();
            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(fakeTransactions.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(fakeTransactions.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(fakeTransactions.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(fakeTransactions.GetEnumerator());
            return mockSet.Object;
        }

        public static Mock<DbSet<T>> ToDbSetMock<T>(this IEnumerable<T> data) where T : class
        {
            var fakeTransactions = data.AsQueryable();
            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(fakeTransactions.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(fakeTransactions.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(fakeTransactions.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(fakeTransactions.GetEnumerator());
            return mockSet;
        }
    }
}
