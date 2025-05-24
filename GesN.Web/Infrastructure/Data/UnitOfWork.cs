using GesN.Web.Interfaces;
using System.Data;

namespace GesN.Web.Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private bool _disposed;
        private IDbTransaction _transaction;
        private int _transactionCount;

        public IDbConnection Connection { get; }
        public IDbTransaction Transaction => _transaction;

        public UnitOfWork(IDbConnection connection)
        {
            Connection = connection;
            if (Connection.State != ConnectionState.Open)
            {
                Connection.Open();
            }
        }

        public void BeginTransaction()
        {
            if (_transaction == null)
            {
                _transaction = Connection.BeginTransaction();
                _transactionCount = 1;
            }
            else
            {
                _transactionCount++;
            }
        }

        public void Commit()
        {
            try
            {
                if (_transaction != null && --_transactionCount == 0)
                {
                    _transaction.Commit();
                    _transaction.Dispose();
                    _transaction = null;
                }
            }
            catch
            {
                Rollback();
                throw;
            }
        }

        public void Rollback()
        {
            try
            {
                if (_transaction != null)
                {
                    _transaction.Rollback();
                    _transaction.Dispose();
                    _transaction = null;
                    _transactionCount = 0;
                }
            }
            finally
            {
                _transaction?.Dispose();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_transaction != null)
                    {
                        Rollback();
                    }
                    Connection?.Dispose();
                }
                _disposed = true;
            }
        }
    }
} 