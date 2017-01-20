﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using POESKillTree.Model.Builds;
using POESKillTree.Model.Items;
using POESKillTree.Model.Serialization;
using POESKillTree.SkillTreeFiles;
using POESKillTree.Utils;
using POESKillTree.Utils.UrlProcessing;

namespace UnitTests.UrlProcessing
{
    [TestClass]
    public class UrlProcessingTests
    {
        private static AbstractPersistentData _persistentData;
        private static SkillTree _tree;

        // These URLs represents same build from #427
        //private static string PathofexileBuildUrlFromApp
            //=> "http://www.pathofexile.com/passive-skill-tree/AAAABAMBAAFvDXwOSA-rD8QRDxEvEVARlhV-FdcV7Ra_GkgbJR1PIG4i9CSLKgsqOCy_LOE1uTY9Ow07fD1fRwZJE0lRSbFLrkyzUDBSU1S9VmNW9VxAXGtd8l9qYqxjQ2nYbAhsC2yMbRlwUnBWdZ51_XzwfeN-oX_GhEiEb4auh8uJ4It6joqP-pAbkQeTH5MnlouXlZfQl_SaE52qoS-io6crpzSnm6xmrJi0DLTFtUi4yrk-u_y-isHFwzrDbcrTzxXQH9DQ1ELVudfP2HbZW9tu29Tb59-K4vfmWOkC6rrrY-wY74jv6_DV8Yry4fQo9vz31_k3";
        private static string PathofexileBuildUrlFromSite
            => "http://www.pathofexile.com/passive-skill-tree/AAAABAMBAAFvDXwOSA-rD8QRDxEvEVARlhV-FdcV7Ra_GkgbJR1PIG4i9CSLKgsqOCy_LOE1uTY9Ow07fD1fRwZJE0lRSbFLrkyzUDBSU1S9VmNW9VxAXGtd8l9qYqxjQ2nYbAhsC2yMbRlwUnBWdZ51_XzwfeN-oX_GhEiEb4auh8uJ4It6joqP-pAbkQeTH5MnlouXlZfQl_SaE52qoS-io6crpzSnm6xmrJi0DLTFtUi4yrk-u_y-isHFwzrDbcrTzxXQH9DQ1ELVudfP2HbZW9tu29Tb59-K4vfmWOkC6rrrY-wY74jv6_DV8Yry4fQo9vz31_k3";
        private static string PathofexileBuildUrlNoAscFromApp
            => "http://www.pathofexile.com/passive-skill-tree/AAAABAMBAAFvDXwOSA-rD8QRDxEvEVARlhXXFe0WvxslHU8gbiL0JIsqCyo4LL8s4TW5Nj07DTt8PV9HBkkTSVFJsUuuTLNQMFJTVL1WY1b1XEBca13yX2pirGNDbAhsC2yMbRlwUnBWdZ51_X3jf8aESIRvhq6Hy4ngi3qOio_6kBuTH5MnlouXlZfQl_SaE52qoS-io6crpzSnm6xmrJi0DLTFtUi4yrk-vorBxcM6w23K088V0B_Q0NRC1bnXz9h22VvbbtvU2-ffiuL35ljpAuq662PsGO-I7-vw1fGK8uH2_PfX-Tc=";
        private static string PathofexileBuildUrlNoAscFromSite
            => "https://www.pathofexile.com/passive-skill-tree/AAAABAMBAAFvDXwOSA-rD8QRDxEvEVARlhXXFe0WvxslHU8gbiL0JIsqCyo4LL8s4TW5Nj07DTt8PV9HBkkTSVFJsUuuTLNQMFJTVL1WY1b1XEBca13yX2pirGNDbAhsC2yMbRlwUnBWdZ51_X3jf8aESIRvhq6Hy4ngi3qOio_6kBuTH5MnlouXlZfQl_SaE52qoS-io6crpzSnm6xmrJi0DLTFtUi4yrk-vorBxcM6w23K088V0B_Q0NRC1bnXz9h22VvbbtvU2-ffiuL35ljpAuq662PsGO-I7-vw1fGK8uH2_PfX-Tc=";

        private static string PoeplannerBuildUrl
            => "http://poeplanner.com/AAQAAPcTEAB534rpAkuuGyXZW20ZEVA1uacrHU-X9FJTPV-XlUyz0B_v6yBuUDARD6yY99eESFxrw23i9wFvi3oi9GwLkyd_xsHF7Bgsv4_60NBfakcG2-e0xcM6KjhcQFS9Fe2Wi9W5h8unm3WezxXYdrk-rGYs4X3j8uFjQ2wI8YrviPk3189Jsb6KoS8RlpAbcFaaE9vUieDmWJMfSRPK07QM6roNfBXXhq5WYw5IXfIqC3BSoqMPxDt8uMrw1UlROw32_Kc0YqxsjJ2qES_rY7VI1EKEb3X9Fr8PqySLNj1W9dtul9COihpI9Ch88JEHu_wVfn6hadgAAAAAAA==";

        // poedb.tw format is compatible with official skill tree
        private static string PoedbBuildUrl =>
            "http://poedb.tw/us/passive-skill-tree/AAAABAMBAAFvDXwOSA-rD8QRDxEvEVARlhV-FdcV7Ra_GkgbJR1PIG4i9CSLKgsqOCy_LOE1uTY9Ow07fD1fRwZJE0lRSbFLrkyzUDBSU1S9VmNW9VxAXGtd8l9qYqxjQ2nYbAhsC2yMbRlwUnBWdZ51_XzwfeN-oX_GhEiEb4auh8uJ4It6joqP-pAbkQeTH5MnlouXlZfQl_SaE52qoS-io6crpzSnm6xmrJi0DLTFtUi4yrk-u_y-isHFwzrDbcrTzxXQH9DQ1ELVudfP2HbZW9tu29Tb59-K4vfmWOkC6rrrY-wY74jv6_DV8Yry4fQo9vz31_k3";

        private static string FullTree =>
            "https://www.pathofexile.com/passive-skill-tree/AAAABAABAAAGAF4AwQDuAQkBbwGRAbwB0QHcAecCEQJfAtAC4wMAAwQDHgMnA3UDhwOWA-4EBwSHBLEEswTkBS0FPAVCBWYFfQWTBbUF-QYOBiAGIwY5BkkGcAZ3BqAGogceB2MHdQelCC4ISQhnCIkIsQjUCPQJMwlYCZYJoAmqCbsJ2Qn2CpsLYQvBDF8McwzyDPcNHw18DY0NzQ3RDjwOSA5cDo4Pqw_EEFgQexCXEMwQ8BEPERoRLREvEVARgRGWEcIR1RJpEuETTBNQE2wTbRNxE4MTyRPMFAkUIBRNFHEUdRSOFKkUsBT2FSAVUBW4FdcV5xXsFe0V8BX2Ff0WQBZvFqoWvxbzFxwXLxc-F1QX3BfhGDwYVhhdGGUYahiRGNcY2xkuGYYZihmOGbQZ1xnpGf4aOBo-GlUabBqBGo0ajxrkGwEbJRtFG5cbqhutG8gb4BvxG_ocpxzOHNwc5x0UHU8dgx2qHb4dwh3ZHpQe3x7wHwIfGB9BH1sfxyBZIG4gnCE0IVUhYCF2IbAhwCHDIiYigSKvIuIi6iL0IzEjXyPTI_YkbySLJJsknSSqJLAk2CT9JSclPyW8Jd8mPCaIJpUm-CcLJyAnIScvJ6kn7SgqKPopLilPKaUqCyoTKjgqTSpTKlsqjSqYKworUCt4K7YsRiyFLJwspiynLL8s4SzpLPstHy0wLVUtfS2DLYstqC3SLgguIy5TLpQvby-dL8wwWzBxMHcwfDCXMZ4xsDGzMfox-zIBMgkyGDI0Mk4yfjKJMpQy0TN4M4czkjQKNCA0MTSONPc1kjW5NdY17zY9Nok2xTbYNug26TdHN2Y3gTeDN9Q4UziWOQ45UjmKOdQ53TpCOlI6WDqdOq06szrYOuE67TsNOyg7Ozt8O-M8BTwoPC08Szy9PO89Dz0-PV890T3dPeI9_D7PPyc_cz_8QKBBP0FyQXRBh0GWQktCjkMTQzFDNkNUQ2NDyEPnRAREDUQlRIpEnkShRKtE0kT7RQpFR0V8RX5FjkWWRZ1FqUW5RfRGaUZxRrdG10cGR35Ht0fiSHhI50juSQ9JE0kbSVFJsUmySdVKLkp9SphKmkrESshLCkscS1dLeEuuTC1MMkyzTP9NRk2STblN404qTjJObU6fTq5PBE9WT31P81AwUEJQR1FHUUxRYFFsUXRR2VHmUftSKVJTUq9SslLsUzFTNVNSU6VTu1PUVEdUYlSCVK5UvVT-VUtVhVWpVa5VtVXGVdVV1lXgVi1WSFZKVmNW6Fb1VvpXDVcpVytXVFeHV5RXl1fJV9hX4VfiWAdYF1haWGNYdVh3WK5Yr1jlWW1ZvFnzWf5aGlorWlJakVqfWyZboFuvW_RcQFxrXIpc-V1JXWhd8l4TXkVeXV6LXqVewF8EXypfOV8_X2pfmF-wYEFgQ2BlYHNgkWCzYMRg3mEGYSFhUmHiYetiWmJ5YpVirGLsYxdjQ2NwY6dj_WQJZFJkWmSEZJ1ko2SqZK9k52VNZadl8GZUZp5mtmdYZ2dngGebZ6BnvWf8aFhoZWh0aPJp_moBagRqHmo2ajtqQ2qMapNqlWqlaqxq-msXa6xrt2vZa9tsCGwLbEZsjGznbRltN206bWxttG49bmlul26qbwhvJ287b1dvhG-eb_JwUXBScFZwu3FNcXlxhXGhcbByD3Jscqlyw3Nrc21zcHOzdFV0oHSudO108XVOdZ51y3XQdf12EXYrdm92f3aCdqx253b3dwd3U3fXd-J343fld_14DXgZeC94aXh6eK54znjreTl5aHmhefZ6U3p_euZ673sNexR7IHtue3R7jHvDe9d8DnxLfIN8uHy7fNl85X0YfVt9dX3SfeN99X5Zfpp-r36wfsd-4H7ifwJ_K3_Gf-N_-4BWgIqApICugOGBOoFBgayBr4IHghCCHoJTgl6Cm4LHguSDCYNfg7aDvYPMg9uD84P3hEiEU4RvhHeExITFhNmE74UOhTKFUoVghW2Fe4V9hcWGAoY7hmCGroazhs6G0YcThymHQYdlh2qHdofLiECIQohaiFuIj4jtiWuJ04nYieCKBooiiiiKOIqvirOK5Irwi0-LeouMi5mMC4w2jEaMdozPjRmNfY1-jYGNgo2_jjyOZI6Kjr6Ov47pjxqPRo9Pj2CPmY-mj8GP-pAKkA2QEZAbkDOQVZBskNaRK5HOkn2SgJLBktCS85MHkx-TJ5M6k5CTlZOZk6iT_JRclG-Uh5SglLiVBJUFlSCVLpVmlYeVyJXMljKWbJZ0louXBpchl3mXhZeVl5eXtJfQl_SYCphvmK2ZK5oTmheaO5phmmqarprPmuCa8Zsmmy2bWJtdm2qbhpuNm6GbtZvsnDKcpJy-nMSdqp2undmePJ5XnqGexJ7Fns2fAZ8nnz6fgp-In6Ofy5_foEKgQ6CfoSKhL6GkogCiBKIuoj6iQKKjotmi6qOKo-ej76PypAykGaQ5pHikwqXEpcumMqZXpn-mjaaZpqymr6a-puCm66cIpyunMKc0p1WnlKebp5-npafUqAeocqh9qJqooqknqTSpSKluqZSplan6qsSq-KsLq7qr0qxHrGasl6yYrKqsr60zrUqtja3xrhKuPq5QrrOvbK-Nr5uvp6-3r-uwC7B3sJKwq7DYsQWxMLE2sUKxW7GQsbOyGbJwstyzA7MOsz-0DLQatC-0OLTFtNG01LUEtQi1SLWFtfK2LLaGtoq2pLb6txe3IrcwtzG3Prd1t7a31rfZuJO4yrjQuNm5Hbk-uUO5b7l8uZO7Tbueu-O77byavJ-86r0nvTa9gb2Cvea-Or6Avoq-p768vu6_AL-Xv9XAGsBUwGbAdsCawJzApsC_wOPBAMEEwQfBM8FJwXzBgsGLwaDBo8G0wcXB1cHYwfPCc8KOwuzDCcMTwzPDOsNtxBXEWMSCxKLEuMT2xSjFU8WKxq7G2MfKyAzIFMgjyFvIzMjcyPDJPclEyWfKRspKypDKqcrTy73L9cwGzGbMt8y8zRbNmM3qznDOqM8KzxXPMs96z37P3c_p0B_Q0ND10TbR5NH90iHS6NMb02_TftP70_zUI9RC1FLUfNSP1S7VddWB1YvVptW51fjWB9ZY1l3Wita-1y3XfteW18_YC9gk2E3YVNhg2HbYvdkL2RPZW9lf2WHZfNnG2fzaOtpi2mTajdq52sHa3dsC2wvbGttP21XbWdte227bp9vU2-fcI9wy3D3cc9y93Q3dRt1I3V_djN2S3ajd493n3pbe5d7432_fit-Y37Dfst--37_gEuBV4JXgn-DD4XPhiOHb4e_iLOJD4l7iYeKA4q3i1eL341bjauOE45_kIuRR5K3kseTr5Rnli-XP5ebmWOaB5ojnCucP5zrnVOd05-roWuhm6KTo1ejW6QLpRunV6f7qGOpi6rrrCesU6yzrY-uO6-Tr7uv17BjsOOxV7HTsiuyw7MvtIO087T_tQe1E7YPt1-4O7hXueu6Q7w7vTu9Q73rvfO-I74_v6-_98B_wa_DV8PnxbPGK8azxs_H-8h3yHvIv8kHyRfJa8pfynPLS8uHzBvMJ8xHzb_Ob8930cfTG9Ob06fT49Uv1b_ZI9lv2o_au9tr25_b89zL3Tfd996b3vvfB99f4k_ih-Ov5M_k3-WP5Zfm9-cj52_nd-ej6GPqA-tL66_rv-vH7CfuL-6r79fxL_GT8xf0w_WD-Cf4K_h3-Sf5U_mv-h_6P_rP-uv7I_xz_k_-w_9P_3g==";

        private static string FullPoeplannerTree =>
            "http://poeplanner.com/AAQACv0QAAV8AAYAXgDuAQkBbwGRAbwB0QHcAecCEQLQAuMDAAMeAycDdQOHA5YD7gQHBIcEsQSzBOQFLQU8BUIFZgV9BZMFtQX5Bg4GIAYjBjkGSQZwBncGoAaiBx4HYwd1B6UILghJCGcIiQixCNQI9AkzCVgJlgmgCaoJuwn2CpsLYQvBDF8McwzyDPcNHw18DY0NzQ3RDjwOSA5cDo4Pqw_EEFgQexCXEMwQ8BEPERoRLREvEVARgRGWEcIR1RJpEuETTBNQE2wTbRNxE4MTyRPMFAkUIBRNFHEUdRSOFKkUsBT2FSAVUBW4FdcV5xXsFe0V8BX2Ff0WQBZvFqoWvxbzFxwXLxc-F1QX3BfhGDwYVhhdGGUYahiRGNcY2xkuGYYZihmOGbQZ1xnpGf4aOBo-GlUabBqBGo0ajxrkGwEbJRtFG5cbqhutG8gb4BvxG_ocpxzOHNwc5x0UHU8dgx2qHb4d2R6UHt8e8B8CHxgfQR9bH8cgbiCcITQhVSFgIXYhsCHAIcMiJiKBIq8i4iLqIvQjMSNfI9Mj9iSLJJsknSSqJLAk2CT9JSclPyW8Jd8mPCaIJpUm-CcLJyAnIScvJ6kn7SgqKPopLilPKaUqCyoTKjgqTSpTKlsqjSqYKworUCt4K7YsRiyFLJwspiynLL8s4SzpLPstHy0wLVUtfS2DLYstqC3SLgguIy5TLpQvby-dL8wwWzBxMHcwfDCXMZ4xsDGzMfox-zIBMgkyGDI0Mk4yfjKJMpQy0TN4M4czkjQKNCA0MTSONPc1kjW5NdY17zY9Nok2xTbYNug26TdHN2Y3gTeDN9Q4UziWOQ45UjmKOdQ53TpCOlI6WDqdOq06szrYOuE67TsNOyg7Ozt8O-M8BTwoPC08vTzvPQ89Pj1fPdE93T3iPfw-zz8nP3M__ECgQT9BckF0QYdBlkJLQo5DE0MxQzZDVENjQ8hD50QERA1EikSeRKFEq0TSRPtFCkVHRXxFfkWORZZFnUWpRblF9EZpRnFGt0bXRwZHfke3R-JIeEjnSO5JD0kTSRtJUUmxSbJJ1UouSn1KmEqaSsRKyEsKSxxLV0t4S65MLUwyTLNM_01GTZJNuU3jTipOMk5tTp9Ork8ET1ZPfU_zUDBQQlBHUUdRTFFgUWxRdFHZUeZR-1IpUlNSr1KyUuxTMVM1U1JTpVO7U9RUR1RiVIJUrlS9VP5VS1WFValVrlW1VcZV1VXWVeBWLVZIVkpWY1boVvVW-lcNVylXK1dUV4dXlFeXV8lX2FfhV-JYB1haWGNYdVh3WK5Yr1jlWW1ZvFnzWf5aGlorWlJakVqfWyZboFuvW_RcQFxrXIpc-V1JXWhd8l4TXkVeXV6LXqVewF8EXypfOV8_X2pfmF-wYEFgQ2BlYHNgkWDEYQZhIWFSYeJh62JaYnlilWKsYuxjF2NDY3Bjp2P9ZAlkUmRaZIRknWSjZKpkr2TnZU1lp2XwZlRmnma2Z1hnZ2eAZ5tnoGe9Z_xoWGhlaHRo8mn-agFqBGoeajZqO2pDaoxqk2qVaqVqrGr6axdrrGu3a9lr22wIbAtsRmyMbOdtGW03bTptbG20bj1uaW6XbqpvCG8nbztvV2-Eb55v8nBRcFJwVnC7cU1xeXGFcaFxsHIPcmxyqXLDc2tzbXNwc7N0VXSgdK507XTxdU51nnXLddB1_XYRdit2b3Z_doJ2rHbndvd3B3dTd9d343fld_14DXgZeC94aXh6eK54znjreTl5aHmhefZ6U3p_euZ673sNexR7IHt0e4x7w3vXfA58S3yDfLh8u3zZfOV9GH1bfXV90n3jffV-WX6afq9-sH7HfuB-4n8Cfyt_xn_jf_uAVoCKgKSAroDhgTqBQYGsga-CB4IQgh6CU4JegpuCx4LkgwmDX4O2g72DzIPbg_OD94RIhG-Ed4TEhMWE2YTvhQ6FMoVShWCFbYV7hX2FxYYChjuGYIauhrOGzobRhxOHKYdBh2WHaod2h8uIQIhCiFqIW4iPiO2Ja4nTidiJ4IoGiiKKKIo4iq-Ks4rkivCLT4t6i4yLmYwLjDaMRox2jM-NGY19jX6NgY2Cjb-OPI5kjoqOvo6_jumPGo9Gj0-PYI-Zj6aPwY_6kAqQDZARkBuQM5BVkGyQ1pErkc6SfZKAksGS0JLzkweTH5MnkzqTkJOVk5mTqJP8lFyUb5SHlKCUuJUElQWVIJUulWaVh5XIlcyWMpZslnSWi5cGl3mXhZeVl5eXtJfQl_SYCphvmK2ZK5oTmheaO5phmmqaz5rgmvGbJpstm1ibXZtqm4abjZuhm7Wb7JwynKScvpzEnaqdrp3ZnjyeV56hnsSexZ7NnwGfJ58-n4KfiJ-jn8uf36BCoEOgn6EioS-hpKIAogSiLqJAoqOi2aLqo4qj56Pvo_KkGaQ5pHikwqXEpcumV6Z_po2mmaaspr6m4KbrpwinK6cwpzSnVaeUp5unn6elp9SoB6h9qJqooqknqTSpbqmUqZWp-qrEqvirC6vSrEesZqyXrJisqqyvrTOtSq2NrfGuEq4-rlCus69sr42vm6-nr7ev67ALsHewkrCrsNixBbEwsTaxQrGQsbOyGbJwstyzA7MOsz-0DLQatC-0OLTFtNG01LUEtQi1SLWFtfK2LLaGtoq2pLb6txe3IrcwtzG3Prd1t7a31rfZuJO4yrjQuNm5Hbk-uUO5b7l8uZO7Tbueu-O77byavJ-86r0nvTa9gb2Cvea-Or6Avoq-p768vu6_AL-Xv9XAGsBUwGbAdsCawJzApsC_wOPBAMEEwQfBM8FJwYLBi8GgwaPBtMHFwdXB2MHzwnPCjsLswwnDE8MzwzrDbcQVxFjEgsSixLjE9sUoxVPFisauxtjHysgMyBTII8hbyMzI3MjwyT3JRMlnykrKkMqpytPLvcv1zAbMZsy3zLzNFs2YzerOcM6ozwrPFc8yz3rPfs_dz-nQH9DQ0PXRNtHk0f3SIdMb02_TftP70_zUI9RC1FLUfNSP1S7VddWB1YvVptW51fjWB9ZY1orWvtct137XltfP2AvYJNhN2FTYYNh22L3ZC9kT2VvZX9lh2XzZxtn82jraYtpk2o3audrB2t3bAtsL2xrbT9tV21nbXttu26fb1Nvn3CPcMtw93HPcvd0N3UbdSN1f3YzdqN3j3efelt7l3vjfb9-K35jfsN-y377fv-AS4JXgn-DD4XPhiOHb4e_iLOJD4l7iYeKA4tXi9-NW42rjhOOf5CLkUeSt5LHk6-UZ5Yvlz-Xm5ljmgeaI5wrnD-c651TndOfq6FroZuik6NXo1ukC6Ubp1en-6hjqYuq66wnrFOss62Prjuvk6-7r9ewY7DjsVex07IrssOzL7SDtPO0_7UHtRO2D7dfuDu4V7nrvDu9O71Dveu9874jvj-_r8B_wa_DV8PnxbPGK8azxs_H-8h3yHvIv8kHyRfJa8pfynPLh8wbzCfMR82_zm_Pd9HH0xvTm9On0-PVL9W_2SPZb9qP2rvba9uf2_Pcy9033pve-98H31_iT-KH46_kz-Tf5Y_ll-b35yPnb-d356PoY-oD60vrr-u_68fsJ-4v7qvv1_Ev8ZPzF_TD9YP4J_gr-Hf5J_lT-a_6H_o_-s_66_sj_HP-T_7D_0__eAMECXwMECdkdwiBZJG88S0QlWBdgs2Ded-J7boRTlyGarqI-pAymMqavqHKpSKu6sVvBfMpG0ujWXd2S4FXire6Q7_3y0vd9AAAAAAA=";

        private static int TotalPointsUsed => 113;

        // Used skill points +8 Asc points +1 for class root node +1 for ascendancy disk, represented by its own node
        private static int TotalNodesUsed => 123;

        public TestContext TestContext { get; set; }

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            AppData.SetApplicationData(Environment.CurrentDirectory);

            if (ItemDB.IsEmpty())
                ItemDB.Load("Data/ItemDB/GemList.xml", true);
            _persistentData = new BarePersistentData {CurrentBuild = new PoEBuild()};
            _persistentData.EquipmentData = EquipmentData.CreateAsync(_persistentData.Options).Result;

            // This initialization requires a lot of time, so it is reasonable to reuse one instance if possible.
            // However, as some tests may change tree state this field should be used only for methods,
            // that does not depend on a tree instance.
            _tree = SkillTree.CreateAsync(_persistentData).Result;
        }

        [TestCleanup]
        public void Cleanup()
        {
            SkillTree.ClearAssets();
        }

        #region Full tree selected

        [TestMethod]
        public async Task FullPathofexileTreeLoadUnloadTest()
        {
            SkillTree.ClearAssets();
            SkillTree tree = await SkillTree.CreateAsync(_persistentData);
            tree.LoadFromUrl(FullTree);

            var url = new SkillTreeSerializer(tree).ToUrl();

            Assert.AreEqual(FullTree, url);
        }
        
        [TestMethod]
        public async Task FullPoeplannerTreeLoadUnloadTest()
        {
            SkillTree.ClearAssets();
            SkillTree tree = await SkillTree.CreateAsync(_persistentData);
            tree.LoadFromUrl(FullPoeplannerTree);

            var url = new SkillTreeSerializer(tree).ToUrl();

            Assert.AreEqual(FullTree, url);
        }
        
        #endregion

        #region Urls decoding - pathofexile.com

        [TestMethod]
        public void DecodePathofexileUrlTest()
        {
            HashSet<SkillNode> nodes;
            int chartype;
            int asctype;
            SkillTree.DecodeUrl(PathofexileBuildUrlFromSite, out nodes, out chartype, out asctype);

            Assert.AreEqual(TotalNodesUsed, nodes.Count);
        }

        #endregion

        #region Urls decoding - poeplanner.com

        [TestMethod]
        public void DecodePoeplannerUrlTest()
        {
            HashSet<SkillNode> nodes;
            int chartype;
            int asctype;
            SkillTree.DecodeUrl(PoeplannerBuildUrl, out nodes, out chartype, out asctype);

            Assert.AreEqual(TotalNodesUsed, nodes.Count);
        }

        [TestMethod]
        public void DecodePoeplannerUrlWithJewelsTest()
        {
            // Witch - Occultist with 8 points to get jewel socket and socketed with Intuitive Leap
            var urlWithJewels = "http://poeplanner.com/AAQAACITAAAI37CSwY6-ES18g-vuHNyPGgGPGgoCFQIAAAEAAAAAAAAAAA==";

            HashSet<SkillNode> nodes;
            int chartype;
            int asctype;
            SkillTree.DecodeUrl(urlWithJewels, out nodes, out chartype, out asctype);

            Assert.AreEqual(10, nodes.Count);
        }

        [TestMethod]
        public void DecodePoedbUrlTest()
        {
            HashSet<SkillNode> nodes;
            int chartype;
            int asctype;
            SkillTree.DecodeUrl(PoedbBuildUrl, out nodes, out chartype, out asctype);

            Assert.AreEqual(TotalNodesUsed, nodes.Count);
        }

        [TestMethod]
        public void DecodePoeplannerUrlWithAurasTest()
        {
            var urlWithAuras = "http://poeplanner.com/AAQCAAAAIgEBAQACAAfQAQH0AQkACxEADBAACwUJBw0KAwAABwEABgIAAA==";

            HashSet<SkillNode> nodes;
            int chartype;
            int asctype;
            SkillTree.DecodeUrl(urlWithAuras, out nodes, out chartype, out asctype);

            Assert.AreEqual(1, nodes.Count, "Only one class root node expected.");
        }

        [TestMethod]
        public void DecodePoeplannerUrlWithEquipmentTest()
        {
            // Wand + Gloves. No other info
            var urlWithAuras = "http://poeplanner.com/AAQBAAAAAAAkAAAACQDOABQAAAQHAAwAAAAUAQF4AAADAwAAAAAAAAAAAAAA";

            HashSet<SkillNode> nodes;
            int chartype;
            int asctype;
            SkillTree.DecodeUrl(urlWithAuras, out nodes, out chartype, out asctype);

            Assert.AreEqual(1, nodes.Count, "Only one class root node expected.");
        }
        
        #endregion

        #region Urls encoding

        [TestMethod]
        public async Task SaveToUrlTest()
        {
            string expectedUrl = PathofexileBuildUrlFromSite.Split('/').LastOrDefault();

            // Need instance created in current thread for a WPF UI logic
            SkillTree.ClearAssets();
            SkillTree tree = await SkillTree.CreateAsync(_persistentData);
            tree.LoadFromUrl(PathofexileBuildUrlFromSite);

            var actualUrl = tree.Serializer.ToUrl().Split('/').LastOrDefault();

            Assert.AreEqual(expectedUrl, actualUrl);

            SkillTree.ClearAssets();
        }
        
        [TestMethod]
        public async Task SaveToUrlNoAscendancyPointsTest()
        {
            string expectedUrl = PathofexileBuildUrlNoAscFromSite.Split('/').LastOrDefault();

            // Need instance created in current thread for a WPF UI logic
            SkillTree.ClearAssets();
            SkillTree tree = await SkillTree.CreateAsync(_persistentData);
            tree.LoadFromUrl(PathofexileBuildUrlNoAscFromSite);

            var actualUrl = tree.Serializer.ToUrl().Split('/').LastOrDefault();

            Assert.AreEqual(expectedUrl, actualUrl);

            SkillTree.ClearAssets();
        }
        
        [TestMethod]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML", @"..\..\TestBuilds\EmptyBuilds.xml", "TestBuild", DataAccessMethod.Sequential)]
        public void GetCharacterUrlTest()
        {
            string expectedUrl = Convert.ToString(TestContext.DataRow["TreeUrl"], CultureInfo.InvariantCulture);
            byte charType = Convert.ToByte(TestContext.DataRow["CharacterClassId"]);
            byte ascType = Convert.ToByte(TestContext.DataRow["AscendancyClassId"]);

            var actualUrl = SkillTree.GetCharacterUrl(charType, ascType);

            Assert.AreEqual(expectedUrl, actualUrl);
        }

        #endregion

        #region Misc

        [TestMethod]
        public void PointsUsedTest()
        {
            uint actualCount = (uint)BuildConverter.GetUrlDeserializer(PathofexileBuildUrlFromSite).GetPointsCount();

            Assert.AreEqual((uint)TotalPointsUsed, actualCount);
        }

        [TestMethod]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML", @"..\..\TestBuilds\EmptyBuilds.xml", "TestBuild", DataAccessMethod.Sequential)]
        public void GetCharacterClassTest()
        {
            string expectedClass = Convert.ToString(TestContext.DataRow["Class"], CultureInfo.InvariantCulture);
            string targetUrl = Convert.ToString(TestContext.DataRow["TreeUrl"], CultureInfo.InvariantCulture);

            string actualClass = BuildConverter.GetUrlDeserializer(Constants.TreeAddress + targetUrl).GetCharacterClass();

            Assert.AreEqual(expectedClass, actualClass);
        }

        [TestMethod]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML", @"..\..\TestBuilds\EmptyBuilds.xml", "TestBuild", DataAccessMethod.Sequential)]
        public void GetAscendancyClassTest()
        {
            // There are duplicate cases for rows with empty ascendancy class.
            // Consider this if test runs too slow.
            string expectedAscendancyClass = Convert.ToString(TestContext.DataRow["AscendancyClass"],
                CultureInfo.InvariantCulture);

            if (string.IsNullOrEmpty(expectedAscendancyClass))
                expectedAscendancyClass = null;

            string targetUrl = Convert.ToString(TestContext.DataRow["TreeUrl"], CultureInfo.InvariantCulture);
            string actualAscendancyClass = BuildConverter.GetUrlDeserializer(Constants.TreeAddress + targetUrl).GetAscendancyClass();

            Assert.AreEqual(expectedAscendancyClass, actualAscendancyClass);
        }

        [TestMethod]
        public async Task GetPointCountTest()
        {
            // Need instance created in current thread for a WPF UI logic
            SkillTree.ClearAssets();
            SkillTree tree = await SkillTree.CreateAsync(_persistentData);
            tree.LoadFromUrl(PathofexileBuildUrlFromSite);

            var deserializer = BuildConverter.GetUrlDeserializer(PathofexileBuildUrlFromSite);

            var points = tree.GetPointCount();
            var count = points["NormalUsed"] + points["AscendancyUsed"] + points["ScionAscendancyChoices"];

            Assert.AreEqual(count, deserializer.GetPointsCount(true));
        }

        #endregion
    }
}