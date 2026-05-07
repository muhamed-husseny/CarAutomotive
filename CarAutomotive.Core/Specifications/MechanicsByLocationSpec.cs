using CarAutomotive.Core.Entities.Mechanic;
using NetTopologySuite.Geometries;

namespace CarAutomotive.Core.Specifications
{
    public class MechanicsByLocationSpec : BaseSpecification<MechanicProfile>
    {
        public MechanicsByLocationSpec(double lat, double lng, double radiusInKilometers)
            : base(m =>

                m.IsAvailable &&


                m.Location.IsWithinDistance(
                    new Point(lng, lat) { SRID = 4326 },
                    radiusInKilometers * 1000
                )
            )
        {
            AddInclude(m => m.Services);

            AddOrderBy(m => m.Location.Distance(new Point(lng, lat) { SRID = 4326 }));

        }
    }
}
