//
//  Copyright (c) 2015 IronSource. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "IronSource/ISBaseAdapter+Internal.h"

static NSString * const UnityAdsAdapterVersion     = @"4.1.4";
static NSString *  GitHash = @"965bebb53";

//System Frameworks For UnityAds Adapter

@import AdSupport;
@import StoreKit;
@import CoreTelephony;

@interface ISUnityAdsAdapter : ISBaseAdapter

@end
