using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Driven.App.BrandPeriodSalesReporting.Models;


namespace Driven.App.BrandPeriodSalesReporting
{
    public class AutoMapperConfig
    {
        //************************************
        // Common/Advanced Mappings
        //************************************

        //public class PublisherToPublisherViewModelConverter : AutoMapper.ITypeConverter<Publisher, PublisherViewModel>
        //{
        //    public PublisherViewModel Convert(AutoMapper.ResolutionContext context)
        //    {
        //        PublisherViewModel dest = null;
        //        if (context.SourceValue != null)
        //        {
        //            Publisher source = (Publisher)context.SourceValue;
        //            dest = new PublisherViewModel();
        //            dest.PublisherId = source.PublisherId;
        //            dest.PublisherTypeId = source.PublisherTypeId;
        //            dest.PublisherTypeName = (source.PublisherType != null) ? source.PublisherType.Description : string.Empty;
        //            dest.PublisherName = source.PublisherName;
        //            dest.Rank = source.Rank;
        //            dest.EstDate = source.EstDate;
        //            dest.EstValuation = source.EstValuation;
        //        }

        //        return dest;
        //    }
        //}

        //AutoMapper.Mapper.CreateMap<Publisher, PublisherViewModel>()
        //    .ForMember(dest => dest.PublisherId, o => o.MapFrom(src => src.PublisherId))
        //    .ForMember(dest => dest.PublisherName, o => o.MapFrom(src => src.PublisherName));

        //AutoMapper.Mapper.CreateMap<Publisher, PublisherViewModel>();

        //AutoMapper.Mapper.CreateMap<Publisher, PublisherViewModel>().ConvertUsing<PublisherToPublisherViewModelConverter>();

        //AutoMapper.Mapper.CreateMap<PublisherViewModel, Publisher>()
        //    .ForMember(dest => dest.PublisherTypeId, o => o.MapFrom(src => src.PublisherTypeId));


        public static void RegisterAutoMapper()
        {
            //AutoMapper.Mapper.CreateMap<Product, ProductViewModel>();
            //AutoMapper.Mapper.CreateMap<ProductViewModel, Product>();
        }
    }
}
