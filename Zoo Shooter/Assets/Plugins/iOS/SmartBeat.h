//
//  SmartBeat.h
//  SmartBeat
//
//  Created by FROSK, Inc. on 2013/11/05.
//  Copyright (c) 2013年 FROSK. All rights reserved.
//
/*!
 @header SmartBeat
 The SmartBeat API provides an initilization interface and configuration interface.
 Use these APIs to enable SmartBeat functionality.
 Please be sure to include the following frameworks in your project to use SmartBeat.<br>
 <ul>
 <li>CoreLocation.framework</li>
 <li>SystemConfiguration.framework</li>
 <li>CoreTelephony.framework</li>
 </ul>
 */


#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

/*!
 @class SmartBeatConfig
 @abstract Configuration to initialize SmartBeat
 */
@interface SmartBeatConfig : NSObject

/*!
 @property apiKey
 @abstract The API key for your application.  You can find your API key on the settings screen of the SmartBeat web console.
 */
@property(nonatomic, copy, readwrite) NSString *apiKey;

/*!
 @property enabled
 @abstract NO if saving/sending Crash/Exception/Captured image shall be disabled by default.
 */
@property(nonatomic, readwrite) BOOL enabled;

/*!
 @property useMachHandler
 @abstract If YES, Marc exception handler is used. If NO, BSD signal handler is used. This property is effective only on iOS 8.0 and above.
 */
@property(nonatomic, readwrite) BOOL useMachHandler;

/*!
 @method configWithApiKey:
 @abstract Returns a config object created by using a given API key.
 @param apiKey The API key for your application.  You can find your API key on the settings screen of the SmartBeat web console.
 @result A config object created by using a given API key.
 */
+ (instancetype)configWithApiKey:(NSString *)apiKey;

@end

/*!
 @class SmartBeat
 @abstract SmartBeat object
 */
@interface SmartBeat : NSObject

/*!
 @method startWithApiKey:
 @abstract Create and initilize a new SmartBeat instance.  This method must be called before calling other SmartBeat methods.
 @param apiKey The API key for your application.  You can find your API key on the settings screen of the SmartBeat web console.
 @result Returns the newly initialized SmartBeat object or nil on error.
 */
+ (SmartBeat *) startWithApiKey:(NSString *) apiKey;

/*!
 @method startWithApiKey:withEnabled:
 @abstract Create and initilize a new SmartBeat instance with initial control.  This method must be called before calling other SmartBeat methods.
 @param apiKey The API key for your application.  You can find your API key on the settings screen of the SmartBeat web console.
 @param enabled NO if saving/sending Crash/Exception/Captured image shall be disabled by default.
 @result Returns the newly initialized SmartBeat object or nil on error.
 */
+ (SmartBeat *) startWithApiKey:(NSString *) apiKey withEnabled:(BOOL) enabled;

/*!
 @method startWithConfig:
 @abstract Create and initilize a new SmartBeat instance with initial control.  This method must be called before calling other SmartBeat methods.
 @param config Configuration to initialize SmartBeat
 @result Returns the newly initialized SmartBeat object or nil on error.
 */
+ (SmartBeat *) startWithConfig:(SmartBeatConfig *) config;

/*!
 @method shared
 @abstract Get the SmartBeat instance.  Note that you must initilize the SmartBeat instance by calling "startWithApiKey" before calling this method.
 @result Returns the instance of SmartBeat object if it is already initilized or nil if it is not initilized.
 */
+ (SmartBeat *) shared;

/*!
 @method setExtraData:
 @abstract Set extra data set that will be sent with crash data.
 @param extraData Set of keys/values. A string represented by description method is logged.
 */
- (void) setExtraData: (NSDictionary *) extraData;

/*!
 @method logException:
 @abstract Send exception data handled by application code to the SmartBeat server.
 @param exception The exception that is handled by the application.
 */
- (void) logException: (NSException *) exception;

/*!
 @method logException:withExtraData:
 @abstract Send exception data handled by application code with any extra data to SmartBeat server.
 @param exception The exception that is handled by the application.
 @param extraData The extraData set that should be sent to server with exception data. Note that extraData set by "setExtraData" won't be sent.
 */
- (void) logException: (NSException *) exception withExtraData:(NSDictionary *) extraData;

/*!
 @method leaveBreadcrumb:
 @abstract Leave a breadcrumb.  When a crash occurs, the last 16 breadcrumbs to be set will be sent with the crash report.
 @param breadcrumb A string shall be set as a breadcrumb
 */
- (void) leaveBreadcrumb: (NSString *) breadcrumb;

/*!
 @method enableNSLog
 @abstract Enable sending NSLog contents to SmartBeat server with crash reports.  If this option is enabled the content of the system log is sent in the crash report when a crash occurs.
 */
- (void) enableNSLog;

/*!
 @method enableDebugLog
 @abstract Enable SmartBeat debug log.  If this option is enabled the SmartBeat library will print debug entries to NSLog.
 */
- (void) enableDebugLog;

/*!
 @method enableAutoScreenCapture
 @abstract Enable auto screen capture.  If this option is enabled screenshots will be taken automatically every second and sent to server when a crash occurs.  At most the last 3 screenshots will be sent.
 */
- (void) enableAutoScreenCapture;

/*!
 @method disableAutoScreenCapture
 @abstract Disable auto screen capture.  Call this method to stop automatic screen captures.
 */
- (void) disableAutoScreenCapture;

/*!
 @method setUserId
 @abstract Set user unique identifier.  Any string can be set. If a user ID has been set it will be included in the crash report.
 @param userId A unique user ID string used by your application.
 */
- (void) setUserId:(NSString *) userId;

/*!
 @method logExceptionForUnity:withMessage:withImagePath:
 @abstract This method is not public.  It may be called from your unity guru code.
 */
- (void) logExceptionForUnity: (NSString *) stackTrace withMessage:(NSString *) message withImagePath:(NSString *) imagePath;

/*!
 @method logExceptionForCocos2dJS:withName:withMessage:withAuxData:
 @abstract This method is not public.  It may be called from your Cocos2d-JS guru code.
 */
- (void) logExceptionForCocos2dJS: (NSString *) stackTrace withName:(NSString *) name withMessage:(NSString *) message withAuxData:(NSDictionary *) auxData;

/*!
 @method beforePresentRenderbuffer
 @abstract Call this API right before calling presentRenderbuffer in your rendering loop to take OpenGL screen captures.
 @param view The UIView to be rendered.
 */
- (void) beforePresentRenderbuffer:(UIView *)view;

/*!
 @method afterPresentRenderbuffer
 @abstract Call this API right after calling presentRenderbuffer in your rendering loop when taking OpenGL screen captures.
 */
- (void) afterPresentRenderbuffer;

/*!
 @method getSdkVersion
 @abstract Get current SDK version
 @result returns current SDK version.
 */
- (NSString *)getSdkVersion;

/*!
 @method enable
 @abstract Enable saving/sending Crash/Exception/Captured image.
 */
- (void) enable;

/*!
 @method disable
 @abstract Disable saving/sending Crash/Exception/Captured image.
 */
- (void) disable;

/*!
 @method isEnabled
 @abstract Return if saving/sending Crash/Exception/Captured image is enabled.
 @result YES if saving/sending Crash/Exception/Captured image is enabled, NO otherwise.
 */
- (BOOL) isEnabled;

/*!
 @method addSensitiveViewTag
 @abstract Add tag of sensitive view. Screen capture is skipped if the view with tag is included in keywindow view hierarchy. NOTE "0" cannot be added because it's default tag id.
 @param tag The tag id of view that screen capture shouldn't be taken.
 */
- (void) addSensitiveViewTag:(NSInteger) tag;

/*!
 @method removeSensitiveViewTag
 @abstract Remove tag of sensitive view.
 @param tag
 */
- (void) removeSensitiveViewTag:(NSInteger) tag;

/*!
 @method whiteListModelForOpenGLES
 @abstract OpenGLES capture will only be enabled for whitelisted model names. Use this interface to register additional model names to the default whitelist. Model names can be acquired by using the sysctlbyname() function with "hw.machine".
 @param model The model name to enable for OpenGL screen capture.
 */
- (void) whiteListModelForOpenGLES:(NSString *)model;

/*!
 @method isWhiteListed
 @abstract Check if this device is listed in white list for OpenGL screen capture.
 @result YES if it is enabled, NO otherwise.
 */
- (BOOL) isWhiteListed;

/*!
 @method notifyRunning
 @abstract Notifies the SmartBeat server that an application is running in the background.
 @discussion
 If your application supports background modes, call this method when the background task is invoked.
 */
- (void) notifyRunning;

/*!
 @method isReadyForDuplicateUserCountPrevention
 @abstract Returns if duplicate user count prevention is available or not.
 @result YES if duplicate user count prevention is available, NO otherwise.
 */
- (BOOL) isReadyForDuplicateUserCountPrevention;

@end

#ifdef __cplusplus
extern "C" {
#endif

/*!
 @method SBLog
 @abstract Puts a log message in the SmartBeat SDK.
 @discussion
 If {@link enableNSLog} is called, logs by this function are ignored and logs by NSLog() are used instead. However, logs by this function are always used on iOS 10 and above.
 
 If you want to use this function instead of NSLog(), it is usefull to define the following macro in your application's Prefix.pch.
 <pre>#define NSLog(...) (NSLog(__VA_ARGS__), SBLog(__VA_ARGS__))</pre>
 Swift can not treatment C style variable length arguments, so please use {@link SBLogv}.
 @param format A log format like printf
 @param ... Log arguments
 */
void SBLog(NSString *format, ...);

/*!
 @method SBLogv
 @abstract Puts a log message in the SmartBeat SDK. (takes va_list)
 @discussion
 If {@link enableNSLog} is called, logs by this function are ignored and logs by NSLog() are used instead. However, logs by this function are always used on iOS 10 and above.
 
 In Swift, please use with getVaList() as below.
 <pre>SBLogv("String: %\@, Integer: %d", getVaList(["text", 1]))</pre>
 @param format A log format like printf
 @param args Log arguments
 */
void SBLogv(NSString *format, va_list args);

#ifdef __cplusplus
}
#endif
