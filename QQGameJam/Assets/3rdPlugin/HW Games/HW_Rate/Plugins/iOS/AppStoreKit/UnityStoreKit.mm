//
//  UnityStoreKit.m
//  Unity-iPhone
//

#import "UnityStoreKit.h"

@implementation UnityStoreKit
#if defined(__cplusplus)
extern "C"{
#endif
    void _goComment()
    {
        if([SKStoreReviewController respondsToSelector:@selector(requestReview)]) {// iOS 10.3 以上支持
            [SKStoreReviewController requestReview];
        }
    }
#if defined(__cplusplus)
}
#endif

@end
