using AutoMapper;
using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWEB
{
    public class AutoMapperConfiguration
    {
        public IMapper Configure()
        {
            MapperConfiguration mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<DomainToViewModelMappingProfile>();
                cfg.AddProfile<ViewModelToDomainMappingProfile>();
            });

            return mapperConfiguration.CreateMapper();
        }
    }
}