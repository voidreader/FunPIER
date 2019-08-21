#import <Foundation/Foundation.h>
#import "SmartBeat.h"

NSString* smartbeat_convertToString(const char* string)
{
    return [NSString stringWithCString: string encoding: NSUTF8StringEncoding];
}

void smartbeat_init_(const char* apikey, bool enabled){
    if(apikey == NULL) return;
    NSString *api_key = smartbeat_convertToString(apikey);
    [SmartBeat startWithApiKey:api_key withEnabled:(enabled ? YES : NO)];
}

void smartbeat_setExtraData_(const char* key, const char* value)
{
    if(key == NULL || value == NULL) return;
    NSString *key_str = smartbeat_convertToString(key);
    NSString *value_str = smartbeat_convertToString(value);
    
    NSDictionary *dic = [NSDictionary dictionaryWithObject: value_str forKey: key_str];
    
    [[SmartBeat shared] setExtraData: dic];
}

void smartbeat_setUserId_(const char* userid)
{
    if(userid == NULL) return;
    NSString *userid_str = smartbeat_convertToString(userid);
    [[SmartBeat shared] setUserId: userid_str];
}

void smartbeat_leaveBreadcrumb_(const char* breadcrumb)
{
    if(breadcrumb == NULL) return;
    NSString *bc = smartbeat_convertToString(breadcrumb);
    [[SmartBeat shared] leaveBreadcrumb:bc];
}

void smartbeat_enableNSLog_()
{
    [[SmartBeat shared] enableNSLog];
}

void smartbeat_enableDebugLog_()
{
    [[SmartBeat shared] enableDebugLog];
}

void smartbeat_logException_(const char* stackTrace, const char* message, const char* imagePath)
{
    NSString *stackTrace_str = smartbeat_convertToString(stackTrace);
    NSString *message_str = smartbeat_convertToString(message);
    NSString *imagePath_str = nil;
    if(imagePath != NULL){
        imagePath_str = smartbeat_convertToString(imagePath);
    }
    
    [[SmartBeat shared] logExceptionForUnity: stackTrace_str withMessage:message_str withImagePath:imagePath_str];
}

void smartbeat_enable_()
{
    [[SmartBeat shared] enable];
}

void smartbeat_disable_()
{
    [[SmartBeat shared] disable];
}

bool smartbeat_isReadyForDuplicateUserCountPrevention_()
{
    return [[SmartBeat shared] isReadyForDuplicateUserCountPrevention];
}

void smartbeat_printlog_(const char* log)
{
    NSString *log_str = smartbeat_convertToString(log);
    NSLog(@"%@", log_str);
    SBLog(@"%@", log_str);
}