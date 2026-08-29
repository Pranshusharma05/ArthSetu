using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArthSetuBackend.Services;
using Microsoft.AspNetCore.Mvc;

namespace ArthSetuBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SyncController : ControllerBase
    {
        private readonly IEnumerable<IGovernmentConnector> _connectors;

        public SyncController(IEnumerable<IGovernmentConnector> connectors)
        {
            _connectors = connectors;
        }

        [HttpPost("nsfdc")]
        public async Task<IActionResult> SyncNsfdc()
        {
            var connector = _connectors.OfType<NsfdcConnectorService>().FirstOrDefault();
            if (connector == null) return NotFound();
            var log = await connector.RunSyncAsync();
            return Ok(log);
        }

        [HttpPost("nbcfdc")]
        public async Task<IActionResult> SyncNbcfdc()
        {
            var connector = _connectors.OfType<NbcfdcConnectorService>().FirstOrDefault();
            if (connector == null) return NotFound();
            var log = await connector.RunSyncAsync();
            return Ok(log);
        }

        [HttpPost("nstfdc")]
        public async Task<IActionResult> SyncNstfdc()
        {
            var connector = _connectors.OfType<NstfdcConnectorService>().FirstOrDefault();
            if (connector == null) return NotFound();
            var log = await connector.RunSyncAsync();
            return Ok(log);
        }

        [HttpPost("ndfdc")]
        public async Task<IActionResult> SyncNdfdc()
        {
            var connector = _connectors.OfType<NdfdcConnectorService>().FirstOrDefault();
            if (connector == null) return NotFound();
            var log = await connector.RunSyncAsync();
            return Ok(log);
        }
        [HttpPost("jansamarth")]
        public async Task<IActionResult> SyncJanSamarth()
        {
            var connector = _connectors.OfType<JanSamarthConnectorService>().FirstOrDefault();
            if (connector == null) return NotFound();
            var log = await connector.RunSyncAsync();
            return Ok(log);
        }

        [HttpPost("nsp")]
        public async Task<IActionResult> SyncNsp()
        {
            var connector = _connectors.OfType<NspConnectorService>().FirstOrDefault();
            if (connector == null) return NotFound();
            var log = await connector.RunSyncAsync();
            return Ok(log);
        }

        [HttpPost("vidyalaxmi")]
        public async Task<IActionResult> SyncVidyalaxmi()
        {
            var connector = _connectors.OfType<VidyalaxmiConnectorService>().FirstOrDefault();
            if (connector == null) return NotFound();
            var log = await connector.RunSyncAsync();
            return Ok(log);
        }

        [HttpPost("mudra")]
        public async Task<IActionResult> SyncMudra()
        {
            var connector = _connectors.OfType<MudraConnectorService>().FirstOrDefault();
            if (connector == null) return NotFound();
            var log = await connector.RunSyncAsync();
            return Ok(log);
        }

        [HttpPost("pmegp")]
        public async Task<IActionResult> SyncPmegp()
        {
            var connector = _connectors.OfType<PmegpConnectorService>().FirstOrDefault();
            if (connector == null) return NotFound();
            var log = await connector.RunSyncAsync();
            return Ok(log);
        }

        [HttpPost("pmvishwakarma")]
        public async Task<IActionResult> SyncPmVishwakarma()
        {
            var connector = _connectors.OfType<PmVishwakarmaConnectorService>().FirstOrDefault();
            if (connector == null) return NotFound();
            var log = await connector.RunSyncAsync();
            return Ok(log);
        }

        [HttpPost("pmfme")]
        public async Task<IActionResult> SyncPmfme()
        {
            var connector = _connectors.OfType<PmfmeConnectorService>().FirstOrDefault();
            if (connector == null) return NotFound();
            var log = await connector.RunSyncAsync();
            return Ok(log);
        }
        [HttpPost("all")]
        public async Task<IActionResult> SyncAll()
        {
            var logs = new List<Models.SourceSyncLog>();
            foreach (var connector in _connectors)
            {
                var log = await connector.RunSyncAsync();
                logs.Add(log);
            }
            return Ok(logs);
        }
    }
}
