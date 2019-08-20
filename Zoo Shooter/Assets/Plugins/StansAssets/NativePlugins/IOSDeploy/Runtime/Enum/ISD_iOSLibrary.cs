using System.ComponentModel;

namespace SA.iOS.XCode
{

    public enum ISD_iOSLibrary
    {
#if !UNITY_WSA
        [DescriptionAttribute("CarrierBundleUtilities.tbd")]
        CarrierBundleUtilities,
        [DescriptionAttribute("IOABPLib.tbd")]
        IOABPLib,
        [DescriptionAttribute("libAccessibility.tbd")]
        libAccessibility,
        [DescriptionAttribute("libacmobileshim.tbd")]
        libacmobileshim,
        [DescriptionAttribute("libafc.tbd")]
        libafc,
        [DescriptionAttribute("libamsupport.tbd")]
        libamsupport,
        [DescriptionAttribute("libAppPatch.tbd")]
        libAppPatch,
        [DescriptionAttribute("libarchive.2.tbd")]
        libarchive2,
        [DescriptionAttribute("libarchive.tbd")]
        libarchive,
        [DescriptionAttribute("libARI.tbd")]
        libARI,
        [DescriptionAttribute("libARIServer.tbd")]
        libARIServer,
        [DescriptionAttribute("libassertion_launchd.tbd")]
        libassertion_launchd,
        [DescriptionAttribute("libate.tbd")]
        libate,
        [DescriptionAttribute("libauthinstall.tbd")]
        libauthinstall,
        [DescriptionAttribute("libAWDProtobufBluetooth.tbd")]
        libAWDProtobufBluetooth,
        [DescriptionAttribute("libAWDProtobufFacetime.tbd")]
        libAWDProtobufFacetime,
        [DescriptionAttribute("libAWDProtobufFacetimeiMessage.tbd")]
        libAWDProtobufFacetimeiMessage,
        [DescriptionAttribute("libAWDProtobufGCK.tbd")]
        libAWDProtobufGCK,
        [DescriptionAttribute("libAWDProtobufLocation.tbd")]
        libAWDProtobufLocation,
        [DescriptionAttribute("libAWDSupport.tbd")]
        libAWDSupport,
        [DescriptionAttribute("libAWDSupportFramework.tbd")]
        libAWDSupportFramework,
        [DescriptionAttribute("libAXSafeCategoryBundle.tbd")]
        libAXSafeCategoryBundle,
        [DescriptionAttribute("libAXSpeechManager.tbd")]
        libAXSpeechManager,
        [DescriptionAttribute("libBasebandManager.tbd")]
        libBasebandManager,
        [DescriptionAttribute("libBasebandUSB.tbd")]
        libBasebandUSB,
        [DescriptionAttribute("libbsm.0.tbd")]
        libbsm0,
        [DescriptionAttribute("libbsm.tbd")]
        libbsm,
        [DescriptionAttribute("libbz2.1.0.tbd")]
        libbz210,
        [DescriptionAttribute("libbz2.tbd")]
        libbz2,
        [DescriptionAttribute("libc++.1.tbd")]
        libcPlusPlus1,
        [DescriptionAttribute("libc++.tbd")]
        libcPlusPlus,
        [DescriptionAttribute("libc++abi.tbd")]
        libcPlusPlusAbi,
        [DescriptionAttribute("libc.tbd")]
        libc,
        [DescriptionAttribute("libcharset.1.0.0.tbd")]
        libcharset100,
        [DescriptionAttribute("libcharset.1.tbd")]
        libcharset1,
        [DescriptionAttribute("libcharset.tbd")]
        libcharset,
        [DescriptionAttribute("libChineseTokenizer.tbd")]
        libChineseTokenizer,
        [DescriptionAttribute("libcmph.tbd")]
        libcmph,
        [DescriptionAttribute("libcompression.tbd")]
        libcompression,
        [DescriptionAttribute("libcoretls.tbd")]
        libcoretls,
        [DescriptionAttribute("libcoretls_cfhelpers.tbd")]
        libcoretls_cfhelpers,
        [DescriptionAttribute("libCRFSuite.tbd")]
        libCRFSuite,
        [DescriptionAttribute("libCRFSuite0.12.tbd")]
        libCRFSuite012,
        [DescriptionAttribute("libCTLogHelper.tbd")]
        libCTLogHelper,
        [DescriptionAttribute("libcupolicy.tbd")]
        libcupolicy,
        [DescriptionAttribute("libcurses.tbd")]
        libcurses,
        [DescriptionAttribute("libdbm.tbd")]
        libdbm,
        [DescriptionAttribute("libDHCPServer.A.tbd")]
        libDHCPServerA,
        [DescriptionAttribute("libDHCPServer.tbd")]
        libDHCPServer,
        [DescriptionAttribute("libdl.tbd")]
        libdl,
        [DescriptionAttribute("libdns_services.tbd")]
        libdns_services,
        [DescriptionAttribute("libdscsym.tbd")]
        libdscsym,
        [DescriptionAttribute("libedit.2.tbd")]
        libedit2,
        [DescriptionAttribute("libedit.3.0.tbd")]
        libedit30,
        [DescriptionAttribute("libedit.3.tbd")]
        libedit3,
        [DescriptionAttribute("libedit.tbd")]
        libedit,
        [DescriptionAttribute("libenergytrace.tbd")]
        libenergytrace,
        [DescriptionAttribute("libETLDIAGLoggingDynamic.tbd")]
        libETLDIAGLoggingDynamic,
        [DescriptionAttribute("libETLDLFDynamic.tbd")]
        libETLDLFDynamic,
        [DescriptionAttribute("libETLDLOADCoreDumpDynamic.tbd")]
        libETLDLOADCoreDumpDynamic,
        [DescriptionAttribute("libETLDLOADDynamic.tbd")]
        libETLDLOADDynamic,
        [DescriptionAttribute("libETLDMCDynamic.tbd")]
        libETLDMCDynamic,
        [DescriptionAttribute("libETLDynamic.tbd")]
        libETLDynamic,
        [DescriptionAttribute("libETLEFSDumpDynamic.tbd")]
        libETLEFSDumpDynamic,
        [DescriptionAttribute("libETLSAHDynamic.tbd")]
        libETLSAHDynamic,
        [DescriptionAttribute("libETLTransportDynamic.tbd")]
        libETLTransportDynamic,
        [DescriptionAttribute("libexslt.0.tbd")]
        libexslt0,
        [DescriptionAttribute("libexslt.tbd")]
        libexslt,
        [DescriptionAttribute("libextension.tbd")]
        libextension,
        [DescriptionAttribute("libform.5.4.tbd")]
        libform54,
        [DescriptionAttribute("libform.tbd")]
        libform,
        [DescriptionAttribute("libgcc_s.1.tbd")]
        libgcc_s1,
        [DescriptionAttribute("libgermantok.tbd")]
        libgermantok,
        [DescriptionAttribute("libH5Dynamic.tbd")]
        libH5Dynamic,
        [DescriptionAttribute("libHDLCDynamic.tbd")]
        libHDLCDynamic,
        [DescriptionAttribute("libheimdal-asn1.tbd")]
        libheimdal_asn1,
        [DescriptionAttribute("libiconv.2.4.0.tbd")]
        libiconv240,
        [DescriptionAttribute("libiconv.2.tbd")]
        libiconv2,
        [DescriptionAttribute("libiconv.tbd")]
        libiconv,
        [DescriptionAttribute("libicucore.A.tbd")]
        libicucoreA,
        [DescriptionAttribute("libicucore.tbd")]
        libicucore,
        [DescriptionAttribute("libInFieldCollection.tbd")]
        libInFieldCollection,
        [DescriptionAttribute("libinfo.tbd")]
        libinfo,
        [DescriptionAttribute("libIOAccessoryManager.tbd")]
        libIOAccessoryManager,
        [DescriptionAttribute("libipconfig.tbd")]
        libipconfig,
        [DescriptionAttribute("libipsec.A.tbd")]
        libipsecA,
        [DescriptionAttribute("libipsec.tbd")]
        libipsec,
        [DescriptionAttribute("libktrace.tbd")]
        libktrace,
        [DescriptionAttribute("liblangid.tbd")]
        liblangid,
        [DescriptionAttribute("libLLVM-C.tbd")]
        libLLVM_C,
        [DescriptionAttribute("libLLVM.tbd")]
        libLLVM,
        [DescriptionAttribute("liblockdown.tbd")]
        liblockdown,
        [DescriptionAttribute("liblizma.5.tbd")]
        liblizma5,
        [DescriptionAttribute("liblizma.tbd")]
        liblizma,
        [DescriptionAttribute("libm.tbd")]
        libm,
        [DescriptionAttribute("libmarisa.tbd")]
        libmarisa,
        [DescriptionAttribute("libMatch.1.tbd")]
        libMatch1,
        [DescriptionAttribute("libMatch.tbd")]
        libMatch,
        [DescriptionAttribute("libmav_ipc_router_dynamic.tbd")]
        libmav_ipc_router_dynamic,
        [DescriptionAttribute("libmecab_em.tbd")]
        libmecab_em,
        [DescriptionAttribute("libmecabra.tbd")]
        libmecabra,
        [DescriptionAttribute("libmis.tbd")]
        libmis,
        [DescriptionAttribute("libMobileCheckpoint.tbd")]
        libMobileCheckpoint,
        [DescriptionAttribute("libMobileGestalt.tbd")]
        libMobileGestalt,
        [DescriptionAttribute("libMobileGestaltExtensions.tbd")]
        libMobileGestaltExtensions,
        [DescriptionAttribute("libncurses.5.4.tbd")]
        libncurses54,
        [DescriptionAttribute("libncurses.tbd")]
        libncurses,
        [DescriptionAttribute("libnetwork.tbd")]
        libnework,
        [DescriptionAttribute("libnfshared.tbd")]
        libnfshared,
        [DescriptionAttribute("libobjc.A.tbd")]
        libobjcA,
        [DescriptionAttribute("libobjc.tbd")]
        libobjc,
        [DescriptionAttribute("libomadm.tbd")]
        libomadm,
        [DescriptionAttribute("libPCITransport.tbd")]
        libPCITransport,
        [DescriptionAttribute("libpoll.tbd")]
        libpoll,
        [DescriptionAttribute("libPPM.tbd")]
        libPPM,
        [DescriptionAttribute("libprequelite.tbd")]
        libprequelite,
        [DescriptionAttribute("libproc.tbd")]
        libproc,
        [DescriptionAttribute("libprotobuf.tbd")]
        libprotobuf,
        [DescriptionAttribute("libpthead.tbd")]
        libpthead,
        [DescriptionAttribute("libQLCharts.tbd")]
        libQLCharts,
        [DescriptionAttribute("libresolv.9.tbd")]
        libresolv9,
        [DescriptionAttribute("libresolv.tbd")]
        libresolv,
        [DescriptionAttribute("librpcsvc.tbd")]
        librpcsvc,
        [DescriptionAttribute("libsandbox.1.tbd")]
        libsandbox1,
        [DescriptionAttribute("libsandbox.tbd")]
        libsandbox,
        [DescriptionAttribute("libsp.tbd")]
        libsp,
        [DescriptionAttribute("libspindump.tbd")]
        libspindump,
        [DescriptionAttribute("libsqlite3.0.tbd")]
        libsqlite30,
        [DescriptionAttribute("libsqlite3.tbd")]
        libsqlite3,
        [DescriptionAttribute("libstdc++.6.0.9.tbd")]
        libstdcPlusPlus_609,
        [DescriptionAttribute("libstdc++.6.tbd")]
        libstdcPlusPlus_6,
        [DescriptionAttribute("libstdc++.tbd")]
        libstdcPlusPlus,
        [DescriptionAttribute("libsysdiagnose.tbd")]
        libsysdiagnose,
        [DescriptionAttribute("libSystem.B.tbd")]
        libSystemB,
        [DescriptionAttribute("libSystem.tbd")]
        libSystem,
        [DescriptionAttribute("libsystemstats.tbd")]
        libsystemstats,
        [DescriptionAttribute("libtailspin.tbd")]
        libtailspin,
        [DescriptionAttribute("libTelephonyBasebandBulkUSBDynamic.tbd")]
        libTelephonyBasebandBulkUSBDynamic,
        [DescriptionAttribute("libTelephonyBasebandDynamic.tbd")]
        libTelephonyBasebandDynamic,
        [DescriptionAttribute("libTelephonyDebugDynamic.tbd")]
        libTelephonyDebugDynamic,
        [DescriptionAttribute("libTelephonyIOKitDynamic.tbd")]
        libTelephonyIOKitDynamic,
        [DescriptionAttribute("libTelephonyUSBDynamic.tbd")]
        libTelephonyUSBDynamic,
        [DescriptionAttribute("libTelephonyUtilDynamic.tbd")]
        libTelephonyUtilDynamic,
        [DescriptionAttribute("libThaiTokenizer.tbd")]
        libThaiTokenizer,
        [DescriptionAttribute("libtidy.A.tbd")]
        libtidyA,
        [DescriptionAttribute("libtidy.tbd")]
        libtidy,
        [DescriptionAttribute("libzupdate.tbd")]
        libzupdate,
        [DescriptionAttribute("libutil.tbd")]
        libutil,
        [DescriptionAttribute("libutil1.0.tbd")]
        libutil_10,
        [DescriptionAttribute("libWAPI.tbd")]
        libWAPI,
        [DescriptionAttribute("libxml2.2.tbd")]
        libxlm22,
        [DescriptionAttribute("libxml2.tbd")]
        libxml2,
        [DescriptionAttribute("libxslt.1.tbd")]
        libxslt1,
        [DescriptionAttribute("libxslt.tbd")]
        libxslt,
        [DescriptionAttribute("libz.1.1.3.tbd")]
        libz113,
        [DescriptionAttribute("libz.1.2.5.tbd")]
        libz125,
        [DescriptionAttribute("libz.1.2.8.tbd")]
        libz128,
        [DescriptionAttribute("libz.1.tbd")]
        libz1,
        [DescriptionAttribute("libz.tbd")]
        libz
#endif
    }
}