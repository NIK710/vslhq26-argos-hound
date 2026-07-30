using Microsoft.AspNetCore.Mvc;

namespace ArgosHound.Api.Controllers;

[ApiController]
[Route("demo/destination")]
public sealed class DemoDestinationController : ControllerBase
{
    [HttpGet]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public ContentResult Get() =>
        Content(
            """
            <!doctype html>
            <html lang="en">
            <head>
              <meta charset="utf-8">
              <meta name="viewport" content="width=device-width, initial-scale=1">
              <title>ArgosHound measured demo</title>
              <style>
                body { font: 16px/1.6 system-ui, sans-serif; max-width: 42rem;
                       margin: 5rem auto; padding: 0 1.5rem; color: #172033; }
                aside { padding: 1rem; border: 1px solid #cbd5e1;
                        border-radius: .75rem; background: #f8fafc; }
              </style>
            </head>
            <body>
              <p>ArgosHound demo destination</p>
              <h1>Your campaign link worked.</h1>
              <p>This page stands in for a product demo, portfolio, or project page.</p>
              <aside>
                <strong>Measurement disclosure</strong>
                <p>
                  ArgosHound recorded that this campaign link was opened. This demo
                  does not use cookies, fingerprint your device, identify a source
                  commenter, or collect additional browsing behavior.
                </p>
              </aside>
            </body>
            </html>
            """,
            "text/html; charset=utf-8");
}
