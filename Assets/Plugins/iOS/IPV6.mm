#include <sys/socket.h>
#include <netdb.h>
#include <arpa/inet.h>
#include <err.h>

#define MakeStringCopy(_x_) ( _x_ != NULL && [_x_ isKindOfClass:[NSString class]] ) ? strdup( [_x_ UTF8String] ) : NULL

extern "C" {

const char *ConnectorCheckIPV6(const char *host) {
    if (nil == host) {
        return NULL;
    }
    const char *dataChar = "No";
    struct addrinfo *res0;
    struct addrinfo hints;
    struct addrinfo *res;
    int n, s;
    memset(&hints, 0, sizeof(hints));
    hints.ai_flags = AI_DEFAULT;
    hints.ai_family = PF_UNSPEC;
    hints.ai_socktype = SOCK_STREAM;
    if ((n = getaddrinfo(host, "http", &hints, &res0)) != 0) {
        return NULL;
    }
    struct sockaddr_in6 *addr6;
    NSString *dataStr = NULL;
    char ipbuf[32];
    s = -1;
    for (res = res0; res; res = res->ai_next) {
        if (res->ai_family == AF_INET6) {
            addr6 = (struct sockaddr_in6 *) res->ai_addr;
            dataChar = inet_ntop(AF_INET6, &addr6->sin6_addr, ipbuf, sizeof(ipbuf));
            NSString *temp = [[NSString alloc] initWithCString:(const char *) dataChar encoding:NSASCIIStringEncoding];
            dataStr = temp;
        } else {
            dataStr = NULL;
        }
        break;
    }
    freeaddrinfo(res0);
    NSString *result = dataStr;
    return MakeStringCopy(result);
}

}
