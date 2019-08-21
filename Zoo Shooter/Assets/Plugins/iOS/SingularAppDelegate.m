//
// Copyright (c) 2017 eppz! mobile, Gergely Borbás (SP)
//
// http://www.twitter.com/_eppz
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A
// PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE
// OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

#import "UnityAppController.h"
#import "SingularStateWrapper.h"
#import "Singular.h"


@interface SingularAppDelegate : UnityAppController
@end


IMPL_APP_CONTROLLER_SUBCLASS(SingularAppDelegate)


@implementation SingularAppDelegate

-(BOOL)application:(UIApplication*) application didFinishLaunchingWithOptions:(NSDictionary*) options
{
    [SingularStateWrapper setLaunchOptions:options];
    
    return [super application:application didFinishLaunchingWithOptions:options];
}

- (BOOL)application:(UIApplication *)application
continueUserActivity:(NSUserActivity *)userActivity
 restorationHandler:(void (^)(NSArray<id<UIUserActivityRestoring>> *restorableObjects))restorationHandler{
    
    if(![SingularStateWrapper isSingularLinksEnabled]){
        return NO;
    }
    
    NSString* apiKey = [SingularStateWrapper getApiKey];
    NSString* apiSecret = [SingularStateWrapper getApiSecret];
    void (^singularLinkHandler)(SingularLinkParams*) = [SingularStateWrapper getSingularLinkHandler];
    int shortlinkResolveTimeout = [SingularStateWrapper getShortlinkResolveTimeout];
    
    if(shortlinkResolveTimeout <= 0){
        [Singular startSession:apiKey
                       withKey:apiSecret
               andUserActivity:userActivity
       withSingularLinkHandler:singularLinkHandler];
    } else{
        [Singular startSession:apiKey
                       withKey:apiSecret
               andUserActivity:userActivity
       withSingularLinkHandler:singularLinkHandler
    andShortLinkResolveTimeout:shortlinkResolveTimeout];
    }
    
    return YES;
}


@end
