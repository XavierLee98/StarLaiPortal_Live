﻿using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

// 2023-07-28 add AR Downpayment Cancellation ver 1.0.7
// 2023-10-05 add payment method for sales return ver 1.0.10
// 2023-10-18 add Stock Count ver 1.0.11
// 2023-10-30 add creditnote payment method ver 1.0.12
// 2024-01-04 add inquiry filter status ver 1.0.15

namespace StarLaiPortal.Module.BusinessObjects
{
    public enum DataType
    {
        [XafDisplayName("String")] String = 0,
        [XafDisplayName("Decimal")] Decimal = 1,
        [XafDisplayName("Int")] Int = 2,
        [XafDisplayName("DateTime")] DateTime = 3
    }

    public enum DocTypeList
    {
        [XafDisplayName("Sales Order")] SO = 0,
        [XafDisplayName("Purchase Order")] PO = 1,
        [XafDisplayName("Sales Quotation")] SQ = 2,
        [XafDisplayName("Pick List")] PL = 3,
        [XafDisplayName("Pack List")] PAL = 4,
        [XafDisplayName("Load")] Load = 5,
        [XafDisplayName("Advanced Shipment Notice")] ASN = 6,
        [XafDisplayName("GRN")] GRN = 7,
        [XafDisplayName("Purchase Return Requests")] PRR = 8,
        [XafDisplayName("Purchase Return")] PR = 9,
        [XafDisplayName("Sales Return Requests")] SRR = 10,
        [XafDisplayName("Sales Return")] SR = 11,
        [XafDisplayName("Warehouse Transfer Requests")] WTR = 12,
        [XafDisplayName("Warehouse Transfer")] WT = 13,
        [XafDisplayName("Stock Adjustment Request")] SAR = 14,
        [XafDisplayName("Stock Adjustment")] SA = 15,
        [XafDisplayName("AR Downpayment")] ARD = 16,
        [XafDisplayName("Sales Refund Request")] SRF = 17,
        [XafDisplayName("Sales Refund")] SRefund = 18,
        [XafDisplayName("Delivery Order")] DO = 19,
        [XafDisplayName("Reports")] Reports = 20,
        // Start ver 1.0.7
        [XafDisplayName("AR Downpayment Cancellation")] ARDC = 21,
        // End ver 1.0.7
        // Start ver 1.0.11
        [XafDisplayName("Stock Count Sheet")] STS = 22,
        [XafDisplayName("Stock Count Confirm")] STC = 23
        // End ver 1.0.11
    }

    public enum DocStatus
    {
        [XafDisplayName("Draft")] Draft = 0,
        [XafDisplayName("Submitted")] Submitted = 1,
        [XafDisplayName("Cancelled")] Cancelled = 2,
        [XafDisplayName("Closed")] Closed = 3,
        [XafDisplayName("Pending Post")] PendPost = 4,
        [XafDisplayName("Posted")] Post = 5,
        [XafDisplayName("Open")] Open = 6,
        // Start ver 1.0.11
        [XafDisplayName("Counting")] Counting = 7
        // End ver 1.0.11
    }

    public enum ApprovalStatusType
    {
        Not_Applicable = 0,
        Approved = 1,
        Required_Approval = 2,
        Rejected = 3
    }

    public enum SearchMethod
    {
        AND = 0,
        OR = 1
    }

    public enum SAPPaymentType
    {
        CASH = 0,
        Cheque = 1,
        [XafDisplayName("Credit Card")] Credit = 2,
        [XafDisplayName("Bank Transfer")] Bank = 3
    }

    public enum LabelType
    {
        LH = 0,
        RH = 1,
        Others = 2
    }

    public enum ReportDocType
    {
        ASN = 0,
        [XafDisplayName("Purchase Order")] PO = 1,
        [XafDisplayName("Sales Return Requests")] SRR = 2
    }

    public enum TransferType
    {
        [XafDisplayName("Not Applicable")] NA = 0,
        [XafDisplayName("Warehouse Transfer")] WT = 1,
        [XafDisplayName("Putaway")] PW = 2,
        [XafDisplayName("Bin Transfer")] BT = 3,
        [XafDisplayName("Bundle Transfer")] Bundle = 4
    }

    public enum PrintStatus
    {
        [XafDisplayName("N/A")] NA = 0,
        [XafDisplayName("Printed")] Printed = 1,
    }

    public enum ApprovalActions
    {
        [XafDisplayName("Please Select Action...")] NA = 0,
        [XafDisplayName("Yes")] Yes = 1,
        [XafDisplayName("No")] No = 2
    }

    public enum AdjustmnetCost
    {
        [XafDisplayName("Last Purchase Price")] LPP = 0,
        [XafDisplayName("Zero Cost")] Zero = 1,
    }

    public enum PrintLabelReprint
    {
        [XafDisplayName("No")] No = 0,
        [XafDisplayName("Yes")] Yes = 1
    }

    // Start ver 1.0.10
    public enum SRPaymentMethod
    {
        [XafDisplayName("Exchange")] Exchange = 0,
        [XafDisplayName("Cash Refund")] CashRefund = 1,
        [XafDisplayName("Transfer Refund")] TransferRefund = 2,
        // Start ver 1.0.12
        [XafDisplayName("Credit Note")] CreditNote = 3,
        // End ver 1.0.12
    }
    // End ver 1.0.10

    // Start ver 1.0.15
    public enum InquiryStatus
    {
        [XafDisplayName("N/A")] NA = 0,
        [XafDisplayName("Draft")] Draft = 1,
        [XafDisplayName("Submitted")] Submitted = 2,
    }

    public enum InquiryViewStatus
    {
        [XafDisplayName("All")] All = 0,
        [XafDisplayName("Draft")] Draft = 1,
        [XafDisplayName("Submitted")] Submitted = 2,
        [XafDisplayName("Cancelled")] Cancelled = 3,
        [XafDisplayName("Closed")] Closed = 4,
        [XafDisplayName("Pending Post")] PendPost = 5,
        [XafDisplayName("Posted")] Post = 6,
        [XafDisplayName("Open")] Open = 7,
        [XafDisplayName("Counting")] Counting = 8
    }
    // End ver 1.0.15
}