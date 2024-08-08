using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AREML.EPOD.Core.Entities.Mappings
{
    [PrimaryKey("PlantGroupId","PlantCode")]
    public class PlantGroupPlantMap
    {
        public int PlantGroupId { get; set; }
        
        public string PlantCode { get; set; }
    }
}
