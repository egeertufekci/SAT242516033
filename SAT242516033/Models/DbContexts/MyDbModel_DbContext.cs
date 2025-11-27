using Microsoft.EntityFrameworkCore;

namespace DbContexts;

public class MyDbModel_DbContext(DbContextOptions<MyDbModel_DbContext> options) : DbContext(options)
{
}