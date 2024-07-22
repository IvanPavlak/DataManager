using Moq;
using Microsoft.EntityFrameworkCore;

namespace DataManager.Test;

public static class MockDbSetHelper
{
    public static Mock<DbSet<T>> CreateMockDbSet<T>(IEnumerable<T> data) where T : class
    {
        var queryableData = data.AsQueryable();

        var mockSet = new Mock<DbSet<T>>();

        mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryableData.Provider);
        mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryableData.Expression);
        mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryableData.ElementType);
        mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryableData.GetEnumerator());

        return mockSet;
    }
}