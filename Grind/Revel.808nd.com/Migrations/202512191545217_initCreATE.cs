namespace Revel._808nd.com.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initCreATE : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Addresses",
                c => new
                    {
                        DBKEY_address_id = c.Int(nullable: false, identity: true),
                        active = c.Boolean(nullable: false),
                        city = c.String(),
                        country = c.String(),
                        created_date = c.DateTime(nullable: false),
                        email = c.String(),
                        id = c.Int(nullable: false),
                        name = c.String(),
                        phone_number = c.String(),
                        primary_billing = c.Boolean(nullable: false),
                        primary_shipping = c.Boolean(nullable: false),
                        resource_uri = c.String(),
                        state = c.String(),
                        street_1 = c.String(),
                        street_2 = c.String(),
                        updated_date = c.DateTime(nullable: false),
                        uuid = c.String(),
                        zipcode = c.String(),
                        customer_id = c.Int(nullable: false),
                        Customer_DBKEY_customer_id = c.Int(),
                    })
                .PrimaryKey(t => t.DBKEY_address_id)
                .ForeignKey("dbo.Customers", t => t.Customer_DBKEY_customer_id)
                .Index(t => t.Customer_DBKEY_customer_id);
            
            CreateTable(
                "dbo.Brands",
                c => new
                    {
                        brand_id = c.Int(nullable: false, identity: true),
                        updated_date = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        name = c.String(),
                        company = c.String(),
                        created_date = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        id = c.Int(nullable: false),
                        call_names = c.String(),
                        resource_uri = c.String(),
                        is_fourth_active = c.Boolean(nullable: false),
                        revel_base_url = c.String(),
                        theAddress = c.String(),
                        ResourceUri = c.String(),
                        key_secret = c.String(),
                        fourth_username = c.String(),
                        fourth_password = c.String(),
                        fourth_guid = c.Guid(),
                        fourth_locationID = c.String(),
                        fourth_RevenueCenter = c.String(),
                        fourth_PushByEstablishment = c.Boolean(nullable: false),
                        emergency_contact = c.String(),
                    })
                .PrimaryKey(t => t.brand_id);
            
            CreateTable(
                "dbo.Customers",
                c => new
                    {
                        DBKEY_customer_id = c.Int(nullable: false, identity: true),
                        Active = c.Boolean(),
                        Address = c.String(),
                        BirthDate = c.DateTime(),
                        CcExp = c.String(),
                        CcFirstName = c.String(),
                        CcLast4Digits = c.String(),
                        CcLastName = c.String(),
                        CcNumber = c.String(),
                        City = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        Email = c.String(),
                        ExpDate = c.DateTime(),
                        FirstName = c.String(),
                        RevelId = c.Int(nullable: false),
                        IsVisitor = c.Boolean(nullable: false),
                        LastName = c.String(),
                        LicNumber = c.String(),
                        LoyaltyNumber = c.String(),
                        LoyaltyRefId = c.String(),
                        Notes = c.String(),
                        PhoneNumber = c.String(),
                        Picture = c.String(),
                        RefNumber = c.String(),
                        ResourceUri = c.String(),
                        State = c.String(),
                        TotalPurchases = c.Int(nullable: false),
                        TotalVisits = c.Int(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(),
                        Uuid = c.String(),
                        Zipcode = c.String(),
                        customer_id = c.Int(nullable: false),
                        establishment_id = c.Int(nullable: false),
                        theAddress = c.String(),
                    })
                .PrimaryKey(t => t.DBKEY_customer_id);
            
            CreateTable(
                "dbo.Discounts",
                c => new
                    {
                        DBKEY_discount_id = c.Int(nullable: false, identity: true),
                        active = c.Boolean(nullable: false),
                        application_type = c.Int(nullable: false),
                        apply_to_base_product_only = c.Boolean(nullable: false),
                        apply_to_entire_application_type = c.Boolean(nullable: false),
                        auto_apply = c.Boolean(nullable: false),
                        barcode = c.String(),
                        brand_level = c.Boolean(nullable: false),
                        created_by = c.String(),
                        created_date = c.String(),
                        discount_amount = c.Int(nullable: false),
                        discount_at_item_level = c.Boolean(nullable: false),
                        discount_code = c.Boolean(nullable: false),
                        discount_type = c.Int(nullable: false),
                        display_on_ipad = c.Boolean(nullable: false),
                        effective_from = c.DateTime(),
                        effective_to = c.DateTime(),
                        establishment = c.String(),
                        how_often_apply = c.Int(nullable: false),
                        id = c.Int(nullable: false),
                        lock_enable = c.Boolean(nullable: false),
                        lock_uuid = c.String(),
                        maximum_off = c.Int(nullable: false),
                        minimum_amount = c.Int(nullable: false),
                        name = c.String(),
                        old_taxed_flag = c.Boolean(nullable: false),
                        password_required = c.Boolean(nullable: false),
                        qualification_subtype = c.Int(nullable: false),
                        qualification_type = c.Int(nullable: false),
                        resource_uri = c.String(),
                        taxed = c.Boolean(nullable: false),
                        updated_by = c.String(),
                        updated_date = c.String(),
                        discount_id = c.Int(nullable: false),
                        establishment_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.DBKEY_discount_id);
            
            CreateTable(
                "dbo.Employees",
                c => new
                    {
                        id = c.Int(nullable: false),
                        active = c.Boolean(nullable: false),
                        brand = c.String(),
                        created_by = c.String(),
                        created_date = c.DateTime(),
                        email = c.String(),
                        employee_card = c.String(),
                        employee_start = c.DateTime(),
                        exempt = c.Boolean(nullable: false),
                        external_id = c.String(),
                        failed_login_attempts = c.Int(nullable: false),
                        first_name = c.String(),
                        internal_empl_id = c.String(),
                        last_name = c.String(),
                        locked_account = c.Boolean(nullable: false),
                        mileage_reimbursement = c.String(),
                        password_history = c.String(),
                        phone_number = c.String(),
                        pin = c.String(),
                        resource_uri = c.String(),
                        updated_by = c.String(),
                        updated_date = c.DateTime(),
                        user = c.String(),
                        FourthEmpNo = c.String(),
                        FourthLocation = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.Establishments",
                c => new
                    {
                        DBKEY_establishment_id = c.Int(nullable: false, identity: true),
                        NumberOfMinutesAfterOpenThatIsLate = c.Int(nullable: false),
                        establishment_id = c.Int(nullable: false),
                        is_fourth_active = c.Boolean(nullable: false),
                        theAddress = c.String(),
                        fourth_locationID = c.String(),
                        address = c.String(),
                        brand = c.String(),
                        email = c.String(),
                        name = c.String(),
                        resource_uri = c.String(),
                        location_email = c.String(),
                        time_zone = c.String(),
                        effective_from = c.DateTime(nullable: false),
                        id = c.String(),
                        db_brand_id = c.Int(nullable: false),
                        RevelOrganiationName = c.String(),
                    })
                .PrimaryKey(t => t.DBKEY_establishment_id);
            
            CreateTable(
                "dbo.CashupNotifiers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NotificationAddress = c.String(),
                        DBKEY_establishment_id = c.Int(nullable: false),
                        Enabled = c.Boolean(nullable: false),
                        UniversalContact = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Establishments", t => t.DBKEY_establishment_id, cascadeDelete: true)
                .Index(t => t.DBKEY_establishment_id);
            
            CreateTable(
                "dbo.OpeningHours",
                c => new
                    {
                        OpeningHoursID = c.Int(nullable: false, identity: true),
                        Day = c.Int(nullable: false),
                        OpeningTime = c.DateTime(),
                        ClosingTime = c.DateTime(),
                        Establishment_DBKEY_establishment_id = c.Int(),
                    })
                .PrimaryKey(t => t.OpeningHoursID)
                .ForeignKey("dbo.Establishments", t => t.Establishment_DBKEY_establishment_id)
                .Index(t => t.Establishment_DBKEY_establishment_id);
            
            CreateTable(
                "dbo.Projections",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProjectionFigure = c.Decimal(nullable: false, precision: 18, scale: 2),
                        _445CalendarWeek_Id = c.Int(),
                        Establishment_DBKEY_establishment_id = c.Int(),
                        ProjectionType_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo._445Calendar", t => t._445CalendarWeek_Id)
                .ForeignKey("dbo.Establishments", t => t.Establishment_DBKEY_establishment_id)
                .ForeignKey("dbo.ProjectionTypes", t => t.ProjectionType_Id)
                .Index(t => t._445CalendarWeek_Id)
                .Index(t => t.Establishment_DBKEY_establishment_id)
                .Index(t => t.ProjectionType_Id);
            
            CreateTable(
                "dbo._445Calendar",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StartDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ProjectionTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.GiftCards",
                c => new
                    {
                        giftcard_id = c.Int(nullable: false, identity: true),
                        address = c.String(),
                        created_by = c.String(),
                        created_date = c.DateTime(nullable: false),
                        customer = c.String(),
                        establishment = c.String(),
                        id = c.Int(nullable: false),
                        initial_value = c.Int(nullable: false),
                        number = c.String(),
                        payment_type = c.Int(nullable: false),
                        remaining_balance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        resource_uri = c.String(),
                        updated_by = c.String(),
                        updated_date = c.DateTime(nullable: false),
                        theAddress = c.String(),
                        LinkingRevelCustomerID = c.Int(nullable: false),
                        LinkingRevelRewardsCardNewID = c.Int(nullable: false),
                        RewardsCardNew_DBKEY_rewardscardnew_id = c.Int(),
                        theCustomer_DBKEY_customer_id = c.Int(),
                    })
                .PrimaryKey(t => t.giftcard_id)
                .ForeignKey("dbo.RewardsCardNews", t => t.RewardsCardNew_DBKEY_rewardscardnew_id)
                .ForeignKey("dbo.Customers", t => t.theCustomer_DBKEY_customer_id)
                .Index(t => t.RewardsCardNew_DBKEY_rewardscardnew_id)
                .Index(t => t.theCustomer_DBKEY_customer_id);
            
            CreateTable(
                "dbo.RewardsCardNews",
                c => new
                    {
                        DBKEY_rewardscardnew_id = c.Int(nullable: false, identity: true),
                        ResourceUri = c.String(),
                        created_by = c.String(),
                        created_date = c.DateTime(nullable: false),
                        current_points = c.Int(nullable: false),
                        customer_revel = c.String(),
                        establishment = c.String(),
                        Revelid = c.Int(nullable: false),
                        number = c.String(),
                        payment_type = c.Int(nullable: false),
                        resource_uri = c.String(),
                        total_points = c.Int(nullable: false),
                        total_purchases = c.Decimal(nullable: false, precision: 18, scale: 2),
                        total_visits = c.Int(nullable: false),
                        updated_by = c.String(),
                        updated_date = c.DateTime(nullable: false),
                        customer_id = c.Int(nullable: false),
                        establishment_id = c.Int(nullable: false),
                        is_vip_card = c.Boolean(),
                        vip_points_refresh = c.Int(nullable: false),
                        vip_points_last_refreshed = c.DateTime(nullable: false),
                        notes = c.String(),
                        days_since_last_visit = c.Int(),
                        yesterdaysTotalPoints = c.Int(),
                        yesterdaysTotalPointsWhenCreated = c.DateTime(),
                        pointsMultiplierLastRun = c.DateTime(),
                        ExpiryDate = c.DateTime(),
                        Active = c.Boolean(),
                        StoresVisted = c.String(),
                        theAddress = c.String(),
                        LoyaltyCardType_id = c.Int(),
                    })
                .PrimaryKey(t => t.DBKEY_rewardscardnew_id)
                .ForeignKey("dbo.LoyaltyCardTypes", t => t.LoyaltyCardType_id)
                .Index(t => t.LoyaltyCardType_id);
            
            CreateTable(
                "dbo.LoyaltyCardTypes",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.HouseAccountPayments",
                c => new
                    {
                        id = c.Int(nullable: false),
                        amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        amount_authorized = c.Decimal(nullable: false, precision: 18, scale: 2),
                        bill = c.Int(nullable: false),
                        card_type = c.Int(nullable: false),
                        cc_first_name = c.String(),
                        cc_last_name = c.String(),
                        change = c.Single(nullable: false),
                        created_date = c.DateTime(nullable: false),
                        customer_id = c.Int(),
                        deleted = c.Boolean(),
                        establishment = c.String(),
                        exchanged = c.Boolean(),
                        executed = c.Boolean(),
                        first_4_cc_digits = c.String(),
                        gratuity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        invoice_transition_date = c.DateTime(),
                        is_billed = c.Boolean(nullable: false),
                        is_paid = c.Boolean(nullable: false),
                        last_4_cc_digits = c.String(),
                        online = c.Boolean(nullable: false),
                        order = c.String(),
                        order_local_id = c.String(),
                        payer_id = c.String(),
                        payment_date = c.DateTime(),
                        payment_type = c.Int(nullable: false),
                        processor_accepted = c.Boolean(),
                        processor_response = c.Boolean(),
                        receipt_email = c.String(),
                        refund_transaction_id = c.String(),
                        refunded = c.Boolean(nullable: false),
                        resource_uri = c.String(),
                        rounding_delta = c.Int(nullable: false),
                        signature_img_url = c.String(),
                        source_type = c.Int(nullable: false),
                        tip = c.Decimal(nullable: false, precision: 18, scale: 2),
                        transaction_captured = c.Boolean(nullable: false),
                        transaction_data = c.String(),
                        transaction_id = c.String(),
                        transaction_status = c.String(),
                        updated_date = c.String(),
                        uuid = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.HouseAccounts",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        balance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        created_date = c.DateTime(nullable: false),
                        customer = c.String(),
                        enabled = c.Boolean(nullable: false),
                        establishment = c.String(),
                        max_limit = c.Decimal(precision: 18, scale: 2),
                        resource_uri = c.String(),
                        updated_date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.MenuFiles",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        bytes = c.Binary(),
                        filename = c.String(),
                        extension = c.String(),
                        url = c.String(),
                        Menu_id = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Menus", t => t.Menu_id)
                .Index(t => t.Menu_id);
            
            CreateTable(
                "dbo.Menus",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        WhenCreated = c.DateTime(nullable: false),
                        WhoUploaded = c.String(),
                        Establishment_DBKEY_establishment_id = c.Int(),
                        MenuType_id = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Establishments", t => t.Establishment_DBKEY_establishment_id)
                .ForeignKey("dbo.MenuTypes", t => t.MenuType_id)
                .Index(t => t.Establishment_DBKEY_establishment_id)
                .Index(t => t.MenuType_id);
            
            CreateTable(
                "dbo.MenuTypes",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.OrderItems",
                c => new
                    {
                        DBKEY_orderitem_id = c.Int(nullable: false, identity: true),
                        catering_complete = c.Boolean(nullable: false),
                        commission = c.String(),
                        cost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        course_number = c.Int(nullable: false),
                        created_by = c.String(),
                        created_date = c.DateTime(),
                        crv_value = c.Decimal(nullable: false, precision: 18, scale: 2),
                        cup_qty = c.Int(nullable: false),
                        cup_weight = c.Decimal(nullable: false, precision: 18, scale: 2),
                        deleted = c.Boolean(nullable: false),
                        dining_option = c.Int(nullable: false),
                        discount = c.String(),
                        discount_amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        discount_reason = c.String(),
                        discount_rule_amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        discount_taxed = c.Boolean(nullable: false),
                        exchange_discount = c.Boolean(),
                        exchanged = c.Boolean(),
                        expedited = c.DateTime(),
                        ervc_type = c.String(),
                        orderitem_id = c.Int(nullable: false),
                        initial_price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        is_cold = c.Boolean(nullable: false),
                        is_coupon = c.Boolean(nullable: false),
                        is_gift = c.Boolean(nullable: false),
                        kitchen_completed = c.DateTime(),
                        modifier_amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        modifier_cost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        modifieritems = c.String(),
                        on_hold = c.Boolean(nullable: false),
                        order = c.String(),
                        order_local_id = c.String(),
                        price = c.Decimal(nullable: false, precision: 18, scale: 10),
                        printed = c.Boolean(nullable: false),
                        product = c.String(),
                        product_name_override = c.String(),
                        quantity = c.Int(nullable: false),
                        resource_uri = c.String(),
                        shared = c.Int(nullable: false),
                        special_request = c.String(),
                        split_parts = c.Int(nullable: false),
                        split_type = c.Int(nullable: false),
                        split_with_seat = c.Int(nullable: false),
                        station = c.String(),
                        tax_amount = c.Decimal(nullable: false, precision: 18, scale: 10),
                        tax_rate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        tax_rebate = c.Int(nullable: false),
                        taxed_flag = c.Boolean(nullable: false),
                        temp_sort = c.Int(nullable: false),
                        updated_by = c.String(),
                        updated_date = c.DateTime(),
                        uuid = c.String(),
                        voided_by = c.String(),
                        voided_date = c.DateTime(),
                        voided_reason = c.String(),
                        weight = c.Decimal(nullable: false, precision: 18, scale: 2),
                        total_price_after_tax = c.Decimal(nullable: false, precision: 18, scale: 2),
                        total_price_after_discount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        parent_order_id = c.Int(nullable: false),
                        product_id = c.Int(nullable: false),
                        discount_id = c.Int(nullable: false),
                        pure_sales = c.Decimal(nullable: false, precision: 18, scale: 10),
                        establishment = c.String(),
                        brand = c.String(),
                        sku = c.String(),
                        db_product_id = c.Int(nullable: false),
                        db_brand_id = c.Int(nullable: false),
                        db_establishment_id = c.Int(nullable: false),
                        establishment_id = c.Int(nullable: false),
                        start_time = c.DateTime(),
                        IsItemWithoutProduct = c.Boolean(),
                        theAddress = c.String(),
                        Order_DBKEY_order_id = c.Int(),
                    })
                .PrimaryKey(t => t.DBKEY_orderitem_id)
                .ForeignKey("dbo.Orders", t => t.Order_DBKEY_order_id)
                .Index(t => t.Order_DBKEY_order_id);
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        DBKEY_order_id = c.Int(nullable: false, identity: true),
                        asap = c.Boolean(),
                        auto_grat_pct = c.Int(),
                        bill_number = c.Int(),
                        bill_parent = c.String(),
                        bills_info = c.String(),
                        bills_type = c.Int(nullable: false),
                        call_name = c.String(),
                        closed = c.Boolean(nullable: false),
                        created_at = c.String(),
                        created_by = c.String(),
                        created_date = c.DateTime(nullable: false),
                        crv_taxed = c.Boolean(nullable: false),
                        crv_value = c.Int(nullable: false),
                        dining_option = c.Int(nullable: false),
                        discount = c.String(),
                        discount_amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        discount_reason = c.String(),
                        discount_rule_amount = c.Decimal(precision: 18, scale: 2),
                        discount_rule_type = c.String(),
                        discount_tax_amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        discount_taxed = c.String(),
                        establishment = c.String(),
                        exchange_discount = c.Boolean(nullable: false),
                        exchanged = c.Boolean(nullable: false),
                        final_total = c.Decimal(nullable: false, precision: 18, scale: 2),
                        gift_reward_data = c.String(),
                        gratuity = c.Decimal(precision: 18, scale: 2),
                        gratuity_type = c.Int(nullable: false),
                        has_delivery_info = c.Boolean(nullable: false),
                        is_discounted = c.String(),
                        is_unpaid = c.String(),
                        order_id = c.Int(),
                        local_id = c.String(),
                        notes = c.String(),
                        notification_email_sent = c.Boolean(nullable: false),
                        notification_text_sent = c.Boolean(nullable: false),
                        number_of_people = c.Int(nullable: false),
                        points_added = c.Int(),
                        points_redeemed = c.Int(),
                        pos_mode = c.String(),
                        prevailing_surcharge = c.Decimal(nullable: false, precision: 18, scale: 2),
                        prevailing_tax = c.Decimal(nullable: false, precision: 18, scale: 2),
                        printed = c.Boolean(nullable: false),
                        remaining_due = c.Int(),
                        resource_uri = c.String(),
                        rounding_delta = c.Int(nullable: false),
                        service_charge = c.Decimal(nullable: false, precision: 18, scale: 2),
                        subtotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        surcharge = c.Decimal(nullable: false, precision: 18, scale: 2),
                        tax = c.Decimal(nullable: false, precision: 18, scale: 2),
                        tax_country = c.String(),
                        tax_rebate = c.Int(nullable: false),
                        updated_by = c.String(),
                        updated_date = c.DateTime(),
                        uuid = c.String(),
                        web_order = c.Boolean(nullable: false),
                        establishment_id = c.Int(nullable: false),
                        db_brand_id = c.Int(nullable: false),
                        theAddress = c.String(),
                    })
                .PrimaryKey(t => t.DBKEY_order_id);
            
            CreateTable(
                "dbo.OrderAllInOnes",
                c => new
                    {
                        DBKEY_id = c.Int(nullable: false, identity: true),
                        asap = c.Boolean(nullable: false),
                        auto_grat_pct = c.Double(),
                        bill_number = c.Int(nullable: false),
                        bill_parent = c.Int(),
                        bills_info = c.String(),
                        bills_type = c.Int(nullable: false),
                        call_name = c.String(),
                        call_number = c.String(),
                        check_sum = c.String(),
                        closed = c.Boolean(nullable: false),
                        created_at = c.String(),
                        created_by = c.String(),
                        created_date = c.DateTime(nullable: false),
                        crv_taxed = c.Boolean(nullable: false),
                        crv_value = c.Decimal(precision: 18, scale: 2),
                        customer = c.String(),
                        customer_birthdate = c.String(),
                        deleted_discounts = c.String(),
                        delivery_address = c.String(),
                        delivery_clock_in = c.String(),
                        delivery_clock_out = c.String(),
                        delivery_distance = c.String(),
                        delivery_duration = c.String(),
                        delivery_employee = c.String(),
                        delivery_estimated_distance = c.String(),
                        dining_option = c.Int(nullable: false),
                        discount = c.String(),
                        discount_amount = c.Decimal(precision: 18, scale: 2),
                        discount_code = c.String(),
                        discount_nontaxable_surcharge_included = c.String(),
                        discount_reason = c.String(),
                        discount_rule_amount = c.String(),
                        discount_rule_type = c.String(),
                        discount_tax_amount = c.String(),
                        discount_tax_amount_included = c.Decimal(precision: 18, scale: 2),
                        discount_taxed = c.String(),
                        discount_total_amount = c.String(),
                        discounted_by = c.String(),
                        establishment = c.String(),
                        exchange_discount = c.String(),
                        exchanged = c.String(),
                        external_sync = c.String(),
                        final_total = c.Decimal(nullable: false, precision: 18, scale: 2),
                        gift_reward_data = c.String(),
                        gratuity = c.Decimal(precision: 18, scale: 2),
                        gratuity_type = c.Int(),
                        ha_applied = c.Boolean(nullable: false),
                        has_delivery_info = c.Boolean(nullable: false),
                        has_history = c.Boolean(nullable: false),
                        has_items = c.Boolean(nullable: false),
                        id = c.Int(nullable: false),
                        invoice_date = c.DateTime(),
                        is_discounted = c.Boolean(nullable: false),
                        is_invoice = c.Boolean(nullable: false),
                        is_readonly = c.Boolean(nullable: false),
                        is_unpaid = c.Boolean(nullable: false),
                        last_updated_at = c.DateTime(),
                        local_id = c.String(),
                        notes = c.String(),
                        notification_email_sent = c.Boolean(nullable: false),
                        notification_text_sent = c.Boolean(nullable: false),
                        number_of_people = c.Int(nullable: false),
                        pickup_time = c.DateTime(),
                        points_added = c.Int(nullable: false),
                        points_redeemed = c.Int(nullable: false),
                        pos_mode = c.String(),
                        prevailing_surcharge = c.Decimal(precision: 18, scale: 2),
                        prevailing_tax = c.Decimal(precision: 18, scale: 2),
                        printed = c.Boolean(nullable: false),
                        registry_data = c.String(),
                        remaining_due = c.Decimal(nullable: false, precision: 18, scale: 2),
                        reporting_id = c.Int(),
                        resource_uri = c.String(),
                        rounding_delta = c.Decimal(precision: 18, scale: 2),
                        sent = c.Boolean(nullable: false),
                        service_charge = c.Decimal(precision: 18, scale: 2),
                        service_fee_tax = c.Decimal(precision: 18, scale: 2),
                        service_fee_taxed = c.Decimal(precision: 18, scale: 2),
                        service_fee_untaxed = c.Decimal(precision: 18, scale: 2),
                        subtotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        surcharge = c.Decimal(nullable: false, precision: 18, scale: 2),
                        surcharge_excluded = c.Double(nullable: false),
                        table_owner = c.String(),
                        tax = c.Decimal(nullable: false, precision: 18, scale: 2),
                        tax_country = c.String(),
                        tax_excluded_amount = c.Double(nullable: false),
                        tax_rebate = c.Double(),
                        taxable_surcharge = c.Double(),
                        taxable_surcharge_excluded = c.Double(),
                        updated_by = c.String(),
                        updated_date = c.DateTime(),
                        uuid = c.String(),
                        vehicle = c.String(),
                        web_order = c.Boolean(nullable: false),
                        theAddress = c.String(),
                    })
                .PrimaryKey(t => t.DBKEY_id);
            
            CreateTable(
                "dbo.Payments",
                c => new
                    {
                        DBKEY_payment_id = c.Int(nullable: false, identity: true),
                        amount = c.Decimal(nullable: false, precision: 18, scale: 10),
                        amount_authorized = c.Decimal(precision: 18, scale: 10),
                        card_type = c.String(),
                        cc_first_name = c.String(),
                        cc_last_name = c.String(),
                        created_by = c.String(),
                        created_date = c.DateTime(),
                        deleted = c.Boolean(),
                        establishment = c.String(),
                        executed = c.Boolean(nullable: false),
                        first_4_cc_digits = c.String(),
                        id = c.Int(nullable: false),
                        last_4_cc_digits = c.String(),
                        order = c.String(),
                        other_payment_type = c.String(),
                        payment_date = c.DateTime(),
                        payment_type = c.Int(),
                        refund_transaction_id = c.String(),
                        updated_date = c.DateTime(),
                        order_id = c.Int(),
                        establishment_id = c.Int(),
                    })
                .PrimaryKey(t => t.DBKEY_payment_id);
            
            CreateTable(
                "dbo.ProductCategories",
                c => new
                    {
                        DBKEY_productcategory_id = c.Int(nullable: false, identity: true),
                        active = c.Boolean(nullable: false),
                        establishment = c.String(),
                        productcategory_id = c.Int(nullable: false),
                        name = c.String(),
                        parent = c.String(),
                        parent_id = c.Int(nullable: false),
                        resource_uri = c.String(),
                        establishment_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.DBKEY_productcategory_id);
            
            CreateTable(
                "dbo.ProductClasses",
                c => new
                    {
                        id = c.Int(nullable: false),
                        active = c.Boolean(nullable: false),
                        admin_class_key = c.String(),
                        brand = c.String(),
                        created_by = c.String(),
                        created_date = c.DateTime(nullable: false),
                        deleted = c.Boolean(nullable: false),
                        exclude_from_discounts = c.Boolean(nullable: false),
                        is_admin_class = c.Boolean(nullable: false),
                        name = c.String(),
                        parent = c.String(),
                        resource_uri = c.String(),
                        sorting = c.Int(nullable: false),
                        updated_by = c.String(),
                        updated_date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        DBKEY_product_id = c.Int(nullable: false, identity: true),
                        active = c.String(),
                        allow_price_override = c.Boolean(nullable: false),
                        attribute_type = c.Int(nullable: false),
                        barcode = c.String(),
                        brand = c.String(),
                        category = c.String(),
                        color_code = c.Int(nullable: false),
                        commission = c.String(),
                        cost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        created_by = c.String(),
                        created_date = c.String(),
                        crv_enabled = c.Boolean(nullable: false),
                        deleted = c.Boolean(nullable: false),
                        description = c.String(),
                        dining_options = c.String(),
                        disable_modifier_popup = c.Boolean(nullable: false),
                        display_on_kiosk = c.Boolean(nullable: false),
                        display_online = c.Boolean(nullable: false),
                        ebt_no = c.Boolean(nullable: false),
                        establishment = c.String(),
                        export = c.Boolean(nullable: false),
                        happy_hour = c.Boolean(nullable: false),
                        product_id = c.Int(nullable: false),
                        is_cold = c.Boolean(nullable: false),
                        is_combo = c.Boolean(nullable: false),
                        is_drink = c.Boolean(nullable: false),
                        kitchen_print_name = c.String(),
                        lock_enable = c.Boolean(nullable: false),
                        max_price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        name = c.String(),
                        preparation_time = c.Int(nullable: false),
                        price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        price_embedded = c.Boolean(nullable: false),
                        product_weight_unit = c.Int(nullable: false),
                        productclass = c.String(),
                        resource_uri = c.String(),
                        rti_combo = c.Boolean(nullable: false),
                        sku = c.String(),
                        sold_by_weight = c.Boolean(nullable: false),
                        sorting = c.Int(nullable: false),
                        tare = c.String(),
                        tax = c.Decimal(precision: 18, scale: 2),
                        tax_class = c.Int(nullable: false),
                        tax_included = c.Boolean(nullable: false),
                        updated_by = c.String(),
                        updated_date = c.String(),
                        uuid = c.String(),
                        variable_pricing = c.Boolean(nullable: false),
                        variable_pricing_by = c.Int(nullable: false),
                        establishment_id = c.Int(nullable: false),
                        productclass_id = c.Int(),
                        tax_id = c.Int(),
                        brand_id = c.Int(),
                        categoryID = c.Int(nullable: false),
                        db_brand_id = c.Int(nullable: false),
                        db_establishment_id = c.Int(nullable: false),
                        theAddress = c.String(),
                    })
                .PrimaryKey(t => t.DBKEY_product_id);
            
            CreateTable(
                "dbo.RewardCardLogs",
                c => new
                    {
                        DB_KEY_id = c.Int(nullable: false, identity: true),
                        created_date = c.DateTime(nullable: false),
                        establishment = c.String(),
                        id = c.Int(nullable: false),
                        order = c.String(),
                        point = c.Decimal(nullable: false, precision: 18, scale: 2),
                        points_by_purchases = c.Decimal(nullable: false, precision: 18, scale: 2),
                        points_by_visits = c.Decimal(nullable: false, precision: 18, scale: 2),
                        purchased = c.Decimal(nullable: false, precision: 18, scale: 2),
                        resource_uri = c.String(),
                        reward_card = c.String(),
                        type_of_change = c.String(),
                        updated_date = c.DateTime(nullable: false),
                        user = c.String(),
                        visit = c.Boolean(nullable: false),
                        theAddress = c.String(),
                        reward_card_id = c.Int(nullable: false),
                        order_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.DB_KEY_id);
            
            CreateTable(
                "dbo.RewardCardPointsTransactionLogs",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        orginal_points_total = c.Int(nullable: false),
                        orginal_points_current = c.Int(nullable: false),
                        new_points_total = c.Int(nullable: false),
                        new_points_current = c.Int(nullable: false),
                        pointsAdded = c.Int(nullable: false),
                        pointSetToRefreshInBucket = c.Int(nullable: false),
                        multiplier = c.Int(nullable: false),
                        card_number = c.String(),
                        WhenCreated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.RewardsCardDailyPoints",
                c => new
                    {
                        id = c.Long(nullable: false, identity: true),
                        date = c.DateTime(nullable: false),
                        total_points_on_date = c.Int(nullable: false),
                        current_points_on_date = c.Int(nullable: false),
                        card_number = c.String(),
                        RewardsCardNew_DBKEY_rewardscardnew_id = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.RewardsCardNews", t => t.RewardsCardNew_DBKEY_rewardscardnew_id)
                .Index(t => t.RewardsCardNew_DBKEY_rewardscardnew_id);
            
            CreateTable(
                "dbo.RewardsPointsMultipliers",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        emailSuffix = c.String(),
                        multiplier = c.Int(nullable: false),
                        active = c.Boolean(nullable: false),
                        expiryDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.ScheduledTaskLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Message = c.String(),
                        FireTime = c.DateTime(),
                        ContainerStartDate = c.DateTime(),
                        ContainerEndDate = c.DateTime(),
                        Detail = c.String(),
                        Result = c.Int(nullable: false),
                        Brand = c.Int(nullable: false),
                        BrandName = c.String(),
                        Establishment = c.Int(nullable: false),
                        EstablishmentName = c.String(),
                        TotalPounds = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalVAT = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalItemCount = c.Int(nullable: false),
                        TotalItemQuantity = c.Int(nullable: false),
                        TotalItemDiscountCount = c.Int(nullable: false),
                        TotalItemDiscountAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalItemDiscountTax = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalItemVoidedCount = c.Int(nullable: false),
                        TotalItemVoidedAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LogType = c.String(),
                        User = c.String(),
                        Notes = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SystemErrors",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        Establishment = c.Int(),
                        Brand = c.Int(),
                        ErrorCode = c.Int(),
                        ErrorDate = c.DateTime(nullable: false),
                        Description = c.String(),
                        Notes = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.SystemLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Type = c.String(),
                        Note = c.String(),
                        WhoTriggered = c.String(),
                        WhenCreated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RewardsCardDailyPoints", "RewardsCardNew_DBKEY_rewardscardnew_id", "dbo.RewardsCardNews");
            DropForeignKey("dbo.OrderItems", "Order_DBKEY_order_id", "dbo.Orders");
            DropForeignKey("dbo.Menus", "MenuType_id", "dbo.MenuTypes");
            DropForeignKey("dbo.MenuFiles", "Menu_id", "dbo.Menus");
            DropForeignKey("dbo.Menus", "Establishment_DBKEY_establishment_id", "dbo.Establishments");
            DropForeignKey("dbo.GiftCards", "theCustomer_DBKEY_customer_id", "dbo.Customers");
            DropForeignKey("dbo.GiftCards", "RewardsCardNew_DBKEY_rewardscardnew_id", "dbo.RewardsCardNews");
            DropForeignKey("dbo.RewardsCardNews", "LoyaltyCardType_id", "dbo.LoyaltyCardTypes");
            DropForeignKey("dbo.Projections", "ProjectionType_Id", "dbo.ProjectionTypes");
            DropForeignKey("dbo.Projections", "Establishment_DBKEY_establishment_id", "dbo.Establishments");
            DropForeignKey("dbo.Projections", "_445CalendarWeek_Id", "dbo._445Calendar");
            DropForeignKey("dbo.OpeningHours", "Establishment_DBKEY_establishment_id", "dbo.Establishments");
            DropForeignKey("dbo.CashupNotifiers", "DBKEY_establishment_id", "dbo.Establishments");
            DropForeignKey("dbo.Addresses", "Customer_DBKEY_customer_id", "dbo.Customers");
            DropIndex("dbo.RewardsCardDailyPoints", new[] { "RewardsCardNew_DBKEY_rewardscardnew_id" });
            DropIndex("dbo.OrderItems", new[] { "Order_DBKEY_order_id" });
            DropIndex("dbo.Menus", new[] { "MenuType_id" });
            DropIndex("dbo.Menus", new[] { "Establishment_DBKEY_establishment_id" });
            DropIndex("dbo.MenuFiles", new[] { "Menu_id" });
            DropIndex("dbo.RewardsCardNews", new[] { "LoyaltyCardType_id" });
            DropIndex("dbo.GiftCards", new[] { "theCustomer_DBKEY_customer_id" });
            DropIndex("dbo.GiftCards", new[] { "RewardsCardNew_DBKEY_rewardscardnew_id" });
            DropIndex("dbo.Projections", new[] { "ProjectionType_Id" });
            DropIndex("dbo.Projections", new[] { "Establishment_DBKEY_establishment_id" });
            DropIndex("dbo.Projections", new[] { "_445CalendarWeek_Id" });
            DropIndex("dbo.OpeningHours", new[] { "Establishment_DBKEY_establishment_id" });
            DropIndex("dbo.CashupNotifiers", new[] { "DBKEY_establishment_id" });
            DropIndex("dbo.Addresses", new[] { "Customer_DBKEY_customer_id" });
            DropTable("dbo.SystemLogs");
            DropTable("dbo.SystemErrors");
            DropTable("dbo.ScheduledTaskLogs");
            DropTable("dbo.RewardsPointsMultipliers");
            DropTable("dbo.RewardsCardDailyPoints");
            DropTable("dbo.RewardCardPointsTransactionLogs");
            DropTable("dbo.RewardCardLogs");
            DropTable("dbo.Products");
            DropTable("dbo.ProductClasses");
            DropTable("dbo.ProductCategories");
            DropTable("dbo.Payments");
            DropTable("dbo.OrderAllInOnes");
            DropTable("dbo.Orders");
            DropTable("dbo.OrderItems");
            DropTable("dbo.MenuTypes");
            DropTable("dbo.Menus");
            DropTable("dbo.MenuFiles");
            DropTable("dbo.HouseAccounts");
            DropTable("dbo.HouseAccountPayments");
            DropTable("dbo.LoyaltyCardTypes");
            DropTable("dbo.RewardsCardNews");
            DropTable("dbo.GiftCards");
            DropTable("dbo.ProjectionTypes");
            DropTable("dbo._445Calendar");
            DropTable("dbo.Projections");
            DropTable("dbo.OpeningHours");
            DropTable("dbo.CashupNotifiers");
            DropTable("dbo.Establishments");
            DropTable("dbo.Employees");
            DropTable("dbo.Discounts");
            DropTable("dbo.Customers");
            DropTable("dbo.Brands");
            DropTable("dbo.Addresses");
        }
    }
}
