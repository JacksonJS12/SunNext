namespace SunNext.Data.Seeding
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using SunNext.Data;
    using SunNext.Data.Models;

    internal class BlogPostSeeder : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            if (dbContext.BlogPosts.Any())
            {
                return; 
            }

            var posts = new HashSet<BlogPost>
            {
                new BlogPost
                {
                    Title = "How Solar Farms are Reshaping Our Energy Future",
                    Summary = "An in-depth exploration of solar farm growth and impact.",
                    Content = @"
<p>Solar farms have emerged as a cornerstone of the global energy transition. Spanning acres and delivering megawatts of clean power, these installations are transforming once-inert landscapes into hubs of renewable energy innovation. Recent technological advancements in photovoltaic panels, grid integration, and predictive analytics mean solar farms deliver more output, more reliably, and more economically than ever before.</p>
<p>At SunNext, we provide real‑time monitoring dashboards that track panel performance, temperature fluctuations, and cloud cover impacts—all in one intuitive interface. Maintenance crews can receive predictive alerts before system degradation occurs, and automated analytics help adjust inverter settings for optimal efficiency.</p>
<p>Beyond mere power generation, solar farms serve as community systems. They bring jobs during construction, offer educational opportunities through outreach, and often beget energy storage and EV charging solutions on-site. In many rural regions, this infrastructure stimulates sustainable development, powering farms, schools, and small businesses.</p>
<p>As governments accelerate decarbonization mandates, solar farms remain essential. Innovations like bifacial panels, single-axis and dual-axis tracking systems, and agrivoltaics (solar with agricultural use) are making solar more adaptable and productive. At SunNext, we're committed to integrating these next-gen technologies into our platform—so asset owners gain full transparency, without complexity.</p>",
                    IsPublished = true,
                    ImagesUrls =
                        "https://www.renogy.com/product_images/uploaded_images/solar-panel-efficiency-over-time.jpg|https://www.renogy.com/product_images/uploaded_images/cost-of-solar-panels-over-time.jpg",
                    CreatedOn = DateTime.UtcNow.AddDays(-30),
                },
                new BlogPost
                {
                    Title = "Innovations in Solar Panel Efficiency and Storage",
                    Summary = "Latest trends boosting solar output and energy storage.",
                    Content = @"
<p>Innovative materials and engineering breakthroughs are pushing solar panel efficiency to new heights. Monocrystalline silicon panels now routinely exceed 22% efficiency, while emerging perovskite tandem panels hold the potential to surpass 30% in lab conditions. These gains translate into higher yield and quicker ROI for solar farm operators.</p>
<p>Equally transformative is energy storage. Lithium-ion batteries are now commonplace, while solid-state and flow battery technologies promise longer life and safer deployment at scale. Facilities combining solar with energy storage can shift solar-generated electricity to peak demand hours, reducing grid strain and saving on peak pricing.</p>
<p>SunNext empowers users to seamlessly integrate storage analytics with generation data. The platform combines real‑time energy flow tracking with SOC (state of charge) prediction, battery health diagnostics, and customizable alert thresholds. Owners can schedule conditional export to the grid or self-consumption transitions based on tariff schedules.</p>
<p>With these advanced capabilities, solar systems can function autonomously, delivering optimized clean energy continuously. Smart pairing of generation, storage, and AI analytics ensures maximum throughput, minimal downtime, and sustainable operation—even under fluctuating environmental conditions.</p>",
                    IsPublished = true,
                    ImagesUrls =
                        "https://ik.imagekit.io/clouglobal/img/wp-content/uploads/2023/12/Scaling-New-Heights-The-Future-of-Renewable-Energy-Shines-Brightly-with-Unveiling-of-Latest-Innovations-symbol-image-credit-CLOU.jpg|https://www.renovablesverdes.com/wp-content/uploads/2025/04/Soluciones-de-paneles-solares-para-entornos-urbanos.jpg|https://batterybusiness.in/wp-content/uploads/2024/12/bb-25122024.jpg|https://www.elcabildo.org/en/wp-content/uploads/2025/07/chinese-company-longi-achieves-breakthrough-34-58-efficiency-with-near-record-hybrid-solar-cell-technology-scaled.jpg",
                    CreatedOn = DateTime.UtcNow.AddDays(-25),
                },
                new BlogPost
                {
                    Title = "Scaling Solar: Managing Performance at Enterprise Level",
                    Summary = "Best practices and strategies for large-scale solar asset management.",
                    Content = @"
<p>Scaling solar infrastructure across multiple locations introduces unique challenges. At enterprise level, managing performance, predictive maintenance, and data integration across arrays requires strategic planning. From remote weather-based scheduling to inverter firmware updates and remote diagnostics, high-scale solar grids benefit from centralized platforms. SunNext's enterprise dashboard integrates all arrays into a unified interface, offering group-level analytics, fault alerting, and performance benchmarking across sites.</p>
<p>For instance, using the platform's AI-powered anomaly detection, you can identify underperforming panels or string arrays before natural degradation becomes visible. Our fleet management module supports remote firmware synchronization, inverter rebalance automation, and scheduled cleaning triggers based on yield reductions. This setup minimizes downtime and keeps energy output at peak.</p>
<p>In addition, cost optimization tools allow finance teams to forecast energy billing, return rates, and ROI per site. Automated export scheduling ensures surplus energy is sold during peak grid demand. SunNext also supports white-label branding for corporate clients looking to provide renewable energy insights to stakeholders or customers. With modular user access control, technicians, site managers, and executives can seamlessly collaborate within the same digital ecosystem.</p>",
                    IsPublished = true,
                    ImagesUrls =
                        "https://www.ul.com/sites/default/files/styles/hero_boxed_width/public/2019-05/Image09_SolarEnergyProject_Caban_021119-Hero-1000x715.jpg?itok=BiEMoerv|https://assets.pvcase.prod.verveagency.com/containers/assets/02-4.jpg/bc1ea05231450cbf0dd0d2a6fed2cd14/02-4.webp",
                    CreatedOn = DateTime.UtcNow.AddDays(-22),
                },

                new BlogPost
                {
                    Title = "Agrivoltaics: Dual Use of Farmland for Crop and Solar",
                    Summary = "Investigating agrivoltaic systems that combine solar arrays with agriculture.",
                    Content = @"
<p>Agrivoltaics is a growing field where solar panels are integrated with crop cultivation on the same land. This dual-use approach improves land efficiency: panels provide shade that reduces evaporation and moderates plant microclimates, while crops help mitigate dust accumulation on modules. Studies show potential yield increases for both energy generation and agricultural productivity, making agrivoltaics ideal for arid and semi-arid regions.</p>
<p>SunNext supports monitoring both energy and environmental parameters in these setups. Temperature, humidity, and irradiance sensors placed across arrays feed into crop-growth models to optimize planting schedules and harvesting phases. With SunNext’s analytics, you can track the net land productivity index, balancing watts per acre with crop yield metrics.</p>
<p>Maintenance scheduling becomes more complex in agrivoltaics — vehicles must navigate between panel rows without damaging crops. SunNext’s mapping engine integrates GIS data and auto-generates service paths. Asset owners also configure variable row spacing, depending on crop height and sunlight tolerance. Through solar-and-agriculture symmetry, landowners can maximize biodiversity, carbon sequestration, and economic returns.</p>",
                    IsPublished = true,
                    ImagesUrls =
                        "https://www.enelnorthamerica.com/content/dam/enel-na/newsroom/agrivoltaics/Friday-2091.jpg| https://civileats.com/wp-content/uploads/2019/01/190122-masachusetts-solar-farm-dual-use-solar-tomato-top2.jpg|https://civileats.com/wp-content/uploads/2021/06/210629-dual-use-solar-agrivoltaics-conservation-land-use-massachusetts-smart-climate-crisis-4-harvest-credit-doe.jpg|https://civileats.com/wp-content/uploads/2021/06/210629-dual-use-solar-agrivoltaics-conservation-land-use-massachusetts-smart-climate-crisis-5-tour-credit-doe.jpg",
                    CreatedOn = DateTime.UtcNow.AddDays(-20),
                },
            };

            await dbContext.BlogPosts.AddRangeAsync(posts);
            await dbContext.SaveChangesAsync();
        }
    }
}