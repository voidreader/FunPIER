//
//  SingularStateWrapper.m
//  Unity-iPhone
//
//  Created by Eyal Rabinovich on 16/04/2019.
//

#import "SingularStateWrapper.h"
#import "SingularLinkParams.h"

@implementation SingularStateWrapper

static NSDictionary* launchOptions;
static bool isSingularLinksEnabled = false;
static NSString* apiKey;
static NSString* apiSecret;
static void(^singularLinkHandler)(SingularLinkParams*);
static int shortlinkResolveTimeout;

+(NSString*)getApiKey{
    return apiKey;
}

+(NSString*)getApiSecret{
    return apiKey;
}

+(void (^)(SingularLinkParams*))getSingularLinkHandler{
    return singularLinkHandler;
}

+(int)getShortlinkResolveTimeout{
    return shortlinkResolveTimeout;
}

+(void)setLaunchOptions:(NSDictionary*) options{
    launchOptions = options;
}

+(NSDictionary*)getLaunchOptions{
    return launchOptions;
}

+(BOOL)enableSingularLinks:(NSString*)key withSecret:(NSString*)secret andHandler:(void (^)(SingularLinkParams*))handler withTimeout:(int)timeoutSec{
    if(key && secret && handler){
        apiKey = key;
        apiSecret = secret;
        singularLinkHandler = handler;
        shortlinkResolveTimeout = timeoutSec;
        
        isSingularLinksEnabled = YES;
    }
    
    return isSingularLinksEnabled;
}

+(BOOL)isSingularLinksEnabled{
    return isSingularLinksEnabled;
}

@end
