using domain.artistresidence.railgunit.com.DataContext;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Revel._808nd.com.Classes.ServiceImplemenations
{
    public class ChargeRoomLogService : IDisposable
    {
        ArtistsResidenceContext _logger;
        public ChargeRoomLogService(ArtistsResidenceContext logger)
        {
            _logger = logger;
        }

        public void LogOrder(ChargeRoomOrderLog log)
        {
            _logOrder(log);
        }

        public void LogOrderItems(IEnumerable<ChargeRoomOrderItemLog> orderItems)
        {
            _logOrderItems(orderItems);
        }

        private void _logOrder(ChargeRoomOrderLog log)
        {
            try
            {
                _logger.ChargeRoomOrderLogs.Add(log);
                _logger.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Could not save order log file", ex);
            }
        }

        private void _logOrderItems(IEnumerable<ChargeRoomOrderItemLog> orderItems)
        {
            try
            {
                _logger.ChargeRoomOrderItemLogs.AddRange(orderItems);
                _logger.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Could not save order item log file", ex);
            }
        }

        /// <summary>
        /// Gets first successful Elina log for any orderURI
        /// </summary>
        /// <param name="orderUri"></param>
        /// <returns></returns>
        public async Task<bool> HasOrderAlreadyBeenLoggedSuccessfully(string orderUri)
        {
            return (await _logger.ChargeRoomOrderLogs.FirstOrDefaultAsync(x => x.OrderResourceUri == orderUri && x.Success)) == null ? false : true;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    _logger = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~ChargeRoomLogService() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion


    }
}
