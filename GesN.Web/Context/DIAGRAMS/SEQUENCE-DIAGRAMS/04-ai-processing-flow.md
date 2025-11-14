# ðŸ¤– SEQUENCE DIAGRAM - IA Processing Flow (Fiscal Document)

## ðŸŽ¯ VisÃ£o Geral
Diagrama de sequÃªncia detalhado mostrando o fluxo completo de processamento inteligente de documentos fiscais com IA, desde o upload atÃ© o prÃ©-preenchimento do formulÃ¡rio de compra. Este fluxo inovador utiliza OCR, Machine Learning e validaÃ§Ã£o humana para automatizar a criaÃ§Ã£o de ordens de compra.

## ðŸ“Š Complexidade do Fluxo
- **ðŸš¨ Alta Complexidade**: AI processing pipeline, human validation workflow, complex data mapping
- **ðŸ‘¥ Participantes**: 10+ system components including AI services
- **ðŸ”„ InteraÃ§Ãµes**: 25+ interactions per document
- **ðŸ¤– AI Integration**: OCR, ML classification, entity extraction
- **ðŸ‘¤ Human Validation**: Manual review and correction workflow

## ðŸŽ¯ Trigger Event
**FiscalDocumentUploaded** (Purchasing Domain) â†’ AI processing pipeline activation

## ðŸ“ Sequence Diagram

```mermaid
sequenceDiagram
    participant UI as ðŸ‘¤ User Interface
    participant PC as ðŸ›’ Purchasing Controller
    participant PS as âš™ï¸ Purchasing Service
    participant FS as ðŸ“ File Storage
    participant AIS as ðŸ¤– AI Service
    participant OCRS as ðŸ“„ OCR Service
    participant MLS as ðŸ§  ML Service
    participant VS as âœ… Validation Service
    participant BGS as âš™ï¸ Background Service
    participant NS as ðŸ”” Notification Service
    participant PR as ðŸ—„ï¸ Purchasing Repository
    participant DB as ðŸ’¾ Database
    
    Note over UI, DB: AI Processing Flow for Fiscal Document (PDF/Image Upload)
    
    %% ==========================================
    %% DOCUMENT UPLOAD AND INITIAL PROCESSING
    %% ==========================================
    
    UI->>PC: POST /PurchaseOrder/UploadFiscalDocument
    Note over UI, PC: User uploads PDF or image file
    
    PC->>PC: ValidateFileFormat(file)
    PC->>PC: ValidateFileSize(file) // Max 10MB
    PC->>PC: ValidateFileType(file) // PDF, JPG, PNG
    
    PC->>PS: UploadFiscalDocumentAsync(file, metadata)
    activate PS
    
    PS->>FS: StoreDocumentAsync(file, documentId)
    activate FS
    FS->>FS: GenerateUniqueDocumentId()
    FS->>FS: SaveToSecureStorage(file, documentId)
    FS->>FS: CreateMetadataRecord(documentId, fileName, fileSize)
    FS-->>PS: DocumentStorageResult (documentId, filePath)
    deactivate FS
    
    PS->>PR: CreateFiscalDocumentRecordAsync(documentId, metadata)
    activate PR
    PR->>DB: INSERT INTO FiscalDocument (status = 'Uploaded')
    DB-->>PR: FiscalDocumentId
    PR-->>PS: Document record created
    deactivate PR
    
    Note over PS: Queue document for AI processing
    PS->>BGS: QueueForAIProcessingAsync(documentId, priority)
    activate BGS
    BGS->>BGS: AddToProcessingQueue(documentId, 'High')
    BGS-->>PS: Queued for processing
    deactivate BGS
    
    PS-->>PC: 200 OK - Document uploaded, processing started
    deactivate PS
    PC-->>UI: Upload successful, processing in background
    
    %% ==========================================
    %% AI PROCESSING PIPELINE
    %% ==========================================
    
    Note over BGS, DB: Background AI Processing Pipeline
    
    BGS->>AIS: ProcessDocumentAsync(documentId)
    activate AIS
    
    AIS->>FS: GetDocumentAsync(documentId)
    activate FS
    FS-->>AIS: DocumentFile (binary data)
    deactivate FS
    
    Note over AIS: Step 1 - Document Type Classification
    AIS->>MLS: ClassifyDocumentTypeAsync(documentFile)
    activate MLS
    MLS->>MLS: RunClassificationModel(documentBytes)
    MLS->>MLS: AnalyzeDocumentStructure()
    MLS-->>AIS: DocumentType (Invoice, Receipt, etc.) + Confidence
    deactivate MLS
    
    AIS->>PR: UpdateDocumentStatusAsync(documentId, "Classified")
    activate PR
    PR->>DB: UPDATE FiscalDocument SET status = 'Classified', document_type
    DB-->>PR: Status updated
    PR-->>AIS: Document status updated
    deactivate PR
    
    Note over AIS: Step 2 - OCR Text Extraction
    AIS->>OCRS: ExtractTextFromDocumentAsync(documentFile, documentType)
    activate OCRS
    
    OCRS->>OCRS: PreprocessImage(documentFile) // Enhance quality
    OCRS->>OCRS: DetectTextRegions() // Find text areas
    OCRS->>OCRS: ExtractRawText() // OCR processing
    OCRS->>OCRS: PostprocessText() // Clean and structure
    
    OCRS-->>AIS: OCRResult (rawText, confidence, textRegions)
    deactivate OCRS
    
    AIS->>PR: UpdateDocumentOCRAsync(documentId, ocrResult)
    activate PR
    PR->>DB: UPDATE FiscalDocument SET ocr_text, ocr_confidence
    DB-->>PR: OCR data saved
    PR-->>AIS: OCR results saved
    deactivate PR
    
    Note over AIS: Step 3 - Data Extraction using ML
    AIS->>MLS: ExtractStructuredDataAsync(rawText, documentType)
    activate MLS
    
    MLS->>MLS: RunNamedEntityRecognition(rawText)
    Note over MLS: Extract key entities from text
    
    MLS->>MLS: ExtractSupplierInfo() // Company name, CNPJ, address
    MLS->>MLS: ExtractItemDetails() // Product names, quantities, prices
    MLS->>MLS: ExtractFinancialData() // Total amount, taxes, discounts
    MLS->>MLS: ExtractDatesAndNumbers() // Issue date, due date, invoice number
    
    MLS->>MLS: ValidateExtractedData() // Business logic validation
    MLS->>MLS: CalculateConfidenceScores() // Per field confidence
    
    MLS-->>AIS: ExtractedData (structured JSON) + FieldConfidences
    deactivate MLS
    
    AIS->>PR: SaveExtractedDataAsync(documentId, extractedData)
    activate PR
    PR->>DB: INSERT INTO AIExtraction (document_id, extracted_data, confidence_scores)
    DB-->>PR: Extraction saved with ID
    PR-->>AIS: Extracted data saved
    deactivate PR
    
    %% ==========================================
    %% CONFIDENCE EVALUATION AND ROUTING
    %% ==========================================
    
    Note over AIS: Evaluate extraction confidence
    AIS->>AIS: CalculateOverallConfidence(fieldConfidences)
    
    alt Overall Confidence >= 85%
        Note over AIS: High confidence - proceed with automatic mapping
        AIS->>AIS: ProceedToAutomaticMapping(extractedData)
        
    else Overall Confidence < 85%
        Note over AIS: Low confidence - requires human validation
        AIS->>NS: QueueForHumanValidationAsync(documentId, extractedData)
        activate NS
        NS->>NS: CreateValidationTask(documentId, extractedData)
        NS->>NS: NotifyValidationTeam(urgency = 'Normal')
        NS-->>AIS: Validation task created
        deactivate NS
        
        AIS->>PR: UpdateDocumentStatusAsync(documentId, "AwaitingValidation")
        activate PR
        PR->>DB: UPDATE FiscalDocument SET status = 'AwaitingValidation'
        DB-->>PR: Status updated
        PR-->>AIS: Status updated
        deactivate PR
        
        Note over AIS: Wait for human validation...
        AIS-->>BGS: Processing paused - awaiting validation
        deactivate AIS
        
        %% ==========================================
        %% HUMAN VALIDATION WORKFLOW
        %% ==========================================
        
        Note over UI, DB: Human Validation Process
        
        UI->>PC: GET /PurchaseOrder/ValidationQueue
        PC->>PS: GetPendingValidationsAsync(userId)
        activate PS
        PS->>PR: GetDocumentsAwaitingValidationAsync()
        activate PR
        PR->>DB: SELECT FiscalDocument WHERE status = 'AwaitingValidation'
        DB-->>PR: Documents requiring validation
        PR-->>PS: Validation queue
        deactivate PR
        PS-->>PC: Pending validations list
        deactivate PS
        PC-->>UI: Validation queue with extracted data
        
        UI->>PC: POST /PurchaseOrder/ValidateExtraction
        Note over UI, PC: User reviews and corrects extracted data
        
        PC->>PS: ValidateExtractionAsync(documentId, correctedData, userId)
        activate PS
        
        PS->>VS: ValidateBusinessRulesAsync(correctedData)
        activate VS
        VS->>VS: ValidateSupplierData() // CNPJ format, etc.
        VS->>VS: ValidateItemData() // Quantities, prices reasonable
        VS->>VS: ValidateFinancialData() // Totals add up correctly
        VS-->>PS: ValidationResult (Success)
        deactivate VS
        
        PS->>PR: SaveValidatedDataAsync(documentId, correctedData, userId)
        activate PR
        PR->>DB: UPDATE AIExtraction SET validated_data, validated_by, validated_at
        DB-->>PR: Validated data saved
        PR-->>PS: Validation saved
        deactivate PR
        
        PS->>BGS: ResumeAIProcessingAsync(documentId)
        activate BGS
        BGS->>AIS: ContinueProcessingWithValidatedData(documentId)
        activate AIS
        
        PS-->>PC: Validation completed successfully
        deactivate PS
        PC-->>UI: Data validated, processing continues
    end
    
    %% ==========================================
    %% DATA MAPPING AND ENTITY RESOLUTION
    %% ==========================================
    
    Note over AIS: Continue with validated or high-confidence data
    AIS->>AIS: GetFinalDataForMapping(documentId) // Validated or original
    
    Note over AIS: Step 4 - Map extracted data to system entities
    AIS->>PS: MapExtractedDataToEntitiesAsync(extractedData)
    activate PS
    
    Note over PS: Map supplier information
    PS->>PR: FindSupplierByIdentifierAsync(cnpj, name)
    activate PR
    PR->>DB: SELECT Supplier WHERE cnpj = ? OR name LIKE ?
    DB-->>PR: Supplier matches or empty
    PR-->>PS: Supplier search results
    deactivate PR
    
    alt Supplier Found
        PS->>PS: MapToExistingSupplier(supplier)
    else Supplier Not Found
        PS->>PS: CreateNewSupplierData(extractedSupplierInfo)
        PS->>PS: FlagForSupplierCreation()
    end
    
    Note over PS: Map ingredient/item information
    loop For each extracted item
        PS->>PR: FindIngredientByNameAsync(itemName)
        activate PR
        PR->>DB: SELECT Ingredient WHERE name LIKE ? OR sku = ?
        DB-->>PR: Ingredient matches or empty
        PR-->>PS: Ingredient search results
        deactivate PR
        
        alt Ingredient Found (Fuzzy Match)
            PS->>PS: MapToExistingIngredient(ingredient, confidence)
        else Ingredient Not Found
            PS->>PS: CreateNewIngredientData(extractedItemInfo)
            PS->>PS: FlagForIngredientCreation()
        end
    end
    
    PS->>PS: CompileMappingResults(supplierMapping, ingredientMappings)
    PS-->>AIS: EntityMappingResult (mapped entities + unmapped items)
    deactivate PS
    
    %% ==========================================
    %% PURCHASE ORDER PRE-POPULATION
    %% ==========================================
    
    Note over AIS: Create purchase order pre-filled data
    AIS->>AIS: CreatePurchaseOrderTemplate(mappingResult, extractedData)
    
    AIS->>PR: SavePrefilledPurchaseOrderAsync(documentId, purchaseOrderTemplate)
    activate PR
    PR->>DB: INSERT INTO PrefilledPurchaseOrder (document_id, prefilled_data)
    DB-->>PR: Prefilled PO saved with ID
    PR-->>AIS: Prefilled purchase order saved
    deactivate PR
    
    AIS->>PR: UpdateDocumentStatusAsync(documentId, "ProcessingComplete")
    activate PR
    PR->>DB: UPDATE FiscalDocument SET status = 'ProcessingComplete', completed_at = NOW()
    DB-->>PR: Document status updated
    PR-->>AIS: Processing marked complete
    deactivate PR
    
    %% ==========================================
    %% NOTIFICATIONS AND USER INTERACTION
    %% ==========================================
    
    Note over AIS: Notify user of completion
    AIS->>NS: NotifyProcessingCompleteAsync(documentId, userId)
    activate NS
    NS->>NS: CreateCompletionNotification()
    NS->>NS: SendEmailToUser(userId, "Document processing complete")
    NS->>NS: CreateInAppNotification(userId, documentId)
    NS-->>AIS: User notified
    deactivate NS
    
    AIS-->>BGS: Document processing completed successfully
    deactivate AIS
    deactivate BGS
    
    %% ==========================================
    %% USER ACCESSES PREFILLED FORM
    %% ==========================================
    
    Note over UI, DB: User accesses prefilled purchase order form
    
    UI->>PC: GET /PurchaseOrder/CreateFromDocument/{documentId}
    PC->>PS: GetPrefilledPurchaseOrderAsync(documentId)
    activate PS
    
    PS->>PR: GetPrefilledDataAsync(documentId)
    activate PR
    PR->>DB: SELECT PrefilledPurchaseOrder WHERE document_id = ?
    DB-->>PR: Prefilled purchase order data
    PR-->>PS: Prefilled data retrieved
    deactivate PR
    
    PS->>PR: GetEntityMappingsAsync(documentId)
    activate PR
    PR->>DB: SELECT entity mappings and confidence scores
    DB-->>PR: Entity mapping results
    PR-->>PS: Mapping results with confidence indicators
    deactivate PR
    
    PS->>PS: PrepareFormDataWithConfidenceIndicators(prefilledData, mappings)
    PS-->>PC: Form data with confidence scores and flags
    deactivate PS
    
    PC-->>UI: Prefilled form with confidence indicators
    
    Note over UI: User reviews form with:
    Note over UI: - Green highlights: High confidence fields
    Note over UI: - Yellow highlights: Medium confidence fields  
    Note over UI: - Red highlights: Low confidence fields
    Note over UI: - New entity alerts: Items requiring creation
    
    %% ==========================================
    %% ERROR HANDLING SCENARIOS
    %% ==========================================
    
    Note over UI, DB: Error Handling Scenarios
    
    alt OCR Processing Failed
        OCRS-->>AIS: OCRError (Unable to extract text)
        AIS->>AIS: LogOCRError(documentId, error)
        AIS->>NS: NotifyAsync(user, "OCR processing failed")
        AIS->>PR: UpdateDocumentStatusAsync(documentId, "OCRFailed")
    end
    
    alt ML Model Confidence Too Low
        MLS-->>AIS: ExtractionResult (confidence < 40%)
        AIS->>AIS: LogLowConfidenceError(documentId, confidence)
        AIS->>NS: CreateManualProcessingTask(documentId)
        AIS->>PR: UpdateDocumentStatusAsync(documentId, "RequiresManualProcessing")
    end
    
    alt File Corruption or Invalid Format
        FS-->>PS: StorageError (File corrupted or invalid)
        PS->>PS: LogFileError(documentId, error)
        PS->>NS: NotifyAsync(user, "File upload failed")
        PS-->>PC: 400 Bad Request - Invalid file
    end
    
    alt Entity Mapping Failures
        PS-->>AIS: MappingResult (many unmapped entities)
        AIS->>NS: NotifyAsync(user, "Manual entity creation required")
        AIS->>PR: FlagForManualEntityCreation(documentId, unmappedEntities)
    end
```

## ðŸŽ¯ Detailed Component Responsibilities

### **ðŸ›’ Purchasing Controller**
```
File Upload Management:
â”œâ”€â”€ ðŸ“ Multi-format file validation (PDF, JPG, PNG)
â”œâ”€â”€ ðŸ”’ File size and security validation (max 10MB)
â”œâ”€â”€ ðŸ“‹ Metadata extraction and validation
â”œâ”€â”€ ðŸš¨ Virus scanning and security checks
â””â”€â”€ ðŸ“Š Upload progress tracking and user feedback

API Endpoints:
â”œâ”€â”€ POST /PurchaseOrder/UploadFiscalDocument
â”œâ”€â”€ GET /PurchaseOrder/ValidationQueue
â”œâ”€â”€ POST /PurchaseOrder/ValidateExtraction
â”œâ”€â”€ GET /PurchaseOrder/CreateFromDocument/{id}
â””â”€â”€ GET /PurchaseOrder/ProcessingStatus/{id}
```

### **ðŸ¤– AI Service**
```
AI Processing Orchestration:
â”œâ”€â”€ ðŸŽ¯ Document type classification coordination
â”œâ”€â”€ ðŸ“„ OCR text extraction management
â”œâ”€â”€ ðŸ§  ML data extraction orchestration
â”œâ”€â”€ ðŸ“Š Confidence score calculation and evaluation
â””â”€â”€ ðŸ”„ Pipeline state management and error handling

Quality Control:
â”œâ”€â”€ âœ… Confidence threshold management (85% default)
â”œâ”€â”€ ðŸŽ¯ Field-level accuracy validation
â”œâ”€â”€ ðŸ“Š Model performance monitoring
â”œâ”€â”€ ðŸ” Error pattern detection and reporting
â””â”€â”€ ðŸ“ˆ Continuous learning from human corrections

Data Orchestration:
â”œâ”€â”€ ðŸ—‚ï¸ Structured data compilation from multiple sources
â”œâ”€â”€ ðŸ”— Cross-field validation and consistency checking
â”œâ”€â”€ ðŸ“‹ Business rule application and validation
â”œâ”€â”€ ðŸŽ¯ Entity resolution and mapping coordination
â””â”€â”€ ðŸ“Š Final output preparation and formatting
```

### **ðŸ“„ OCR Service**
```
Text Extraction Pipeline:
â”œâ”€â”€ ðŸ–¼ï¸ Image preprocessing and enhancement
â”œâ”€â”€ ðŸ“ Text region detection and segmentation
â”œâ”€â”€ ðŸ”¤ Character recognition and text extraction
â”œâ”€â”€ ðŸ“ Post-processing and text cleaning
â””â”€â”€ ðŸ“Š Confidence scoring per text region

Document Handling:
â”œâ”€â”€ ðŸ“„ PDF text layer extraction (when available)
â”œâ”€â”€ ðŸ–¼ï¸ Image-based OCR for scanned documents
â”œâ”€â”€ ðŸ“ Table structure recognition and parsing
â”œâ”€â”€ ðŸŽ¯ Multi-language text detection (PT/EN)
â””â”€â”€ ðŸ“Š Layout analysis and structure preservation

Quality Optimization:
â”œâ”€â”€ ðŸ” Image quality assessment and enhancement
â”œâ”€â”€ ðŸ“ Skew correction and orientation normalization
â”œâ”€â”€ ðŸŽ¯ Noise reduction and clarity improvement
â”œâ”€â”€ ðŸ“Š Multi-pass extraction for low-quality images
â””â”€â”€ âœ… Result validation and confidence calculation
```

### **ðŸ§  ML Service**
```
Machine Learning Models:
â”œâ”€â”€ ðŸ·ï¸ Document Classification Model (Invoice, Receipt, etc.)
â”œâ”€â”€ ðŸ”¤ Named Entity Recognition (NER) for key fields
â”œâ”€â”€ ðŸ“Š Information Extraction for structured data
â”œâ”€â”€ ðŸŽ¯ Entity Resolution for supplier/product mapping
â””â”€â”€ ðŸ“ˆ Confidence Prediction for extraction quality

Entity Extraction:
â”œâ”€â”€ ðŸ¢ Supplier Information (Name, CNPJ, Address, Contact)
â”œâ”€â”€ ðŸ“¦ Item Details (Name, Quantity, Unit Price, Total)
â”œâ”€â”€ ðŸ’° Financial Data (Subtotal, Taxes, Discounts, Total)
â”œâ”€â”€ ðŸ“… Dates and Numbers (Issue Date, Due Date, Invoice Number)
â””â”€â”€ ðŸ“‹ Additional Metadata (Payment Terms, Delivery Info)

Model Management:
â”œâ”€â”€ ðŸ“ˆ Performance monitoring and metric tracking
â”œâ”€â”€ ðŸ”„ Model versioning and deployment management
â”œâ”€â”€ ðŸ“Š Training data collection from human corrections
â”œâ”€â”€ ðŸŽ¯ A/B testing for model improvements
â””â”€â”€ ðŸ“‹ Feedback loop integration for continuous learning
```

### **âœ… Validation Service**
```
Business Rule Validation:
â”œâ”€â”€ ðŸ¢ Supplier Data Validation (CNPJ format, exists in system)
â”œâ”€â”€ ðŸ“¦ Product Data Validation (reasonable quantities, prices)
â”œâ”€â”€ ðŸ’° Financial Data Validation (calculations, tax compliance)
â”œâ”€â”€ ðŸ“… Date Validation (logical dates, business calendar)
â””â”€â”€ ðŸ“‹ Cross-field Consistency (totals match line items)

Data Quality Checks:
â”œâ”€â”€ ðŸ” Format validation (numbers, dates, identifiers)
â”œâ”€â”€ ðŸ“Š Range validation (reasonable values, business limits)
â”œâ”€â”€ ðŸŽ¯ Completeness validation (required fields present)
â”œâ”€â”€ ðŸ”— Relationship validation (FK constraints, dependencies)
â””â”€â”€ ðŸ“ˆ Historical data comparison (price trends, supplier history)

Human Validation Workflow:
â”œâ”€â”€ ðŸ‘¤ Task assignment and priority management
â”œâ”€â”€ ðŸ“‹ Validation interface and user experience
â”œâ”€â”€ ðŸŽ¯ Field-level correction tracking
â”œâ”€â”€ ðŸ“Š Validator performance and accuracy monitoring
â””â”€â”€ ðŸ”„ Feedback collection for model improvement
```

## ðŸ¤– AI Processing Pipeline Details

### **ðŸ“„ Document Classification**
```
Supported Document Types:
â”œâ”€â”€ ðŸ“„ Nota Fiscal EletrÃ´nica (NFe)
â”œâ”€â”€ ðŸ§¾ Nota Fiscal de ServiÃ§o (NFSe)
â”œâ”€â”€ ðŸ“‹ Recibo de Compra
â”œâ”€â”€ ðŸª Cupom Fiscal
â””â”€â”€ ðŸ“‘ Invoice/Receipt (International)

Classification Features:
â”œâ”€â”€ ðŸ“ Layout structure analysis
â”œâ”€â”€ ðŸ”¤ Header text pattern recognition
â”œâ”€â”€ ðŸ·ï¸ Watermark and logo detection
â”œâ”€â”€ ðŸ“Š Field arrangement patterns
â””â”€â”€ ðŸŽ¯ Document format signatures

Confidence Thresholds:
â”œâ”€â”€ ðŸŸ¢ High Confidence: > 90% - Auto-process
â”œâ”€â”€ ðŸŸ¡ Medium Confidence: 70-90% - Additional validation
â”œâ”€â”€ ðŸŸ  Low Confidence: 50-70% - Manual review
â””â”€â”€ ðŸ”´ Very Low: < 50% - Manual processing
```

### **ðŸ”¤ OCR Processing Strategy**
```
Multi-Stage OCR Pipeline:
â”œâ”€â”€ ðŸ“ Stage 1: Layout analysis and region detection
â”œâ”€â”€ ðŸ” Stage 2: Text region classification (header, body, footer)
â”œâ”€â”€ ðŸ”¤ Stage 3: Character recognition with multiple engines
â”œâ”€â”€ ðŸ“ Stage 4: Text reconstruction and formatting
â””â”€â”€ âœ… Stage 5: Quality assessment and validation

OCR Engine Selection:
â”œâ”€â”€ ðŸŽ¯ Primary: Google Vision API (high accuracy)
â”œâ”€â”€ ðŸ”„ Fallback: Tesseract (open source backup)
â”œâ”€â”€ ðŸ“Š Specialized: Table extraction engine
â”œâ”€â”€ ðŸŒ Multi-language: PT-BR optimized models
â””â”€â”€ ðŸ“ˆ Adaptive: Quality-based engine selection

Quality Enhancement:
â”œâ”€â”€ ðŸ–¼ï¸ Image preprocessing (contrast, brightness, noise)
â”œâ”€â”€ ðŸ“ Geometric correction (rotation, skew, perspective)
â”œâ”€â”€ ðŸ” Resolution optimization (upscaling, sharpening)
â”œâ”€â”€ ðŸŽ¯ Region-of-interest focusing
â””â”€â”€ ðŸ“Š Multi-pass extraction for difficult areas
```

### **ðŸ§  Machine Learning Extraction**
```
Named Entity Recognition (NER):
â”œâ”€â”€ ðŸ¢ ORGANIZATION: Company names, suppliers
â”œâ”€â”€ ðŸ‘¤ PERSON: Contact names, signatures
â”œâ”€â”€ ðŸ’° MONEY: Amounts, prices, totals
â”œâ”€â”€ ðŸ“… DATE: Issue dates, due dates, periods
â”œâ”€â”€ ðŸ“¦ PRODUCT: Item names, descriptions, codes
â”œâ”€â”€ ðŸ“ LOCATION: Addresses, delivery locations
â”œâ”€â”€ ðŸ“‹ IDENTIFIER: CNPJ, CPF, invoice numbers
â””â”€â”€ ðŸ“Š QUANTITY: Amounts, units, measurements

Information Extraction Patterns:
â”œâ”€â”€ ðŸ“Š Table structure recognition and parsing
â”œâ”€â”€ ðŸŽ¯ Key-value pair extraction (label: value)
â”œâ”€â”€ ðŸ”— Relationship extraction (item â†’ price â†’ total)
â”œâ”€â”€ ðŸ“‹ List processing (multiple items, line items)
â””â”€â”€ ðŸ§® Mathematical validation (totals, calculations)

Confidence Scoring:
â”œâ”€â”€ ðŸ“Š Field-level confidence (per extracted value)
â”œâ”€â”€ ðŸŽ¯ Context-based confidence (surrounding text quality)
â”œâ”€â”€ ðŸ”— Cross-validation confidence (internal consistency)
â”œâ”€â”€ ðŸ“ˆ Historical confidence (similar document patterns)
â””â”€â”€ ðŸ§® Overall document confidence (weighted average)
```

## ðŸ‘¤ Human Validation Workflow

### **ðŸŽ¯ Validation Interface Design**
```
User Experience Features:
â”œâ”€â”€ ðŸ“Š Side-by-side document view and form
â”œâ”€â”€ ðŸŽ¨ Color-coded confidence indicators
â”œâ”€â”€ ðŸ” Zoom and annotation tools for document review
â”œâ”€â”€ âŒ¨ï¸ Keyboard shortcuts for efficient editing
â””â”€â”€ ðŸ“‹ Bulk validation for multiple documents

Confidence Visualization:
â”œâ”€â”€ ðŸŸ¢ Green: High confidence (> 85%) - minimal review needed
â”œâ”€â”€ ðŸŸ¡ Yellow: Medium confidence (60-85%) - verify accuracy
â”œâ”€â”€ ðŸŸ  Orange: Low confidence (40-60%) - likely needs correction
â”œâ”€â”€ ðŸ”´ Red: Very low confidence (< 40%) - manual entry required
â””â”€â”€ âšª Gray: Not extracted - requires manual input

Validation Tracking:
â”œâ”€â”€ â±ï¸ Time tracking per validation task
â”œâ”€â”€ ðŸ“Š Accuracy metrics per validator
â”œâ”€â”€ ðŸŽ¯ Common error pattern identification
â”œâ”€â”€ ðŸ“ˆ Validator performance analytics
â””â”€â”€ ðŸ”„ Feedback integration for model improvement
```

### **ðŸ“Š Validation Quality Control**
```
Validation Rules:
â”œâ”€â”€ âœ… Required field completeness check
â”œâ”€â”€ ðŸ§® Mathematical validation (totals, tax calculations)
â”œâ”€â”€ ðŸ“… Date logic validation (issue < due date)
â”œâ”€â”€ ðŸ¢ Supplier existence and active status
â””â”€â”€ ðŸ“¦ Product/ingredient existence or creation flags

Quality Metrics:
â”œâ”€â”€ ðŸ“Š Validation accuracy rate (% correct validations)
â”œâ”€â”€ â±ï¸ Average validation time per document
â”œâ”€â”€ ðŸŽ¯ Inter-validator agreement rates
â”œâ”€â”€ ðŸ“ˆ Validation complexity scoring
â””â”€â”€ ðŸ”„ Model improvement impact from validations

Validator Training:
â”œâ”€â”€ ðŸ“š Training materials and best practices
â”œâ”€â”€ ðŸŽ¯ Practice documents with known correct answers
â”œâ”€â”€ ðŸ“Š Performance feedback and coaching
â”œâ”€â”€ ðŸ† Gamification and quality incentives
â””â”€â”€ ðŸ“ˆ Continuous skill development tracking
```

## ðŸ”— Entity Mapping and Resolution

### **ðŸ¢ Supplier Mapping Strategy**
```
Supplier Identification:
â”œâ”€â”€ ðŸŽ¯ Exact CNPJ match (highest priority)
â”œâ”€â”€ ðŸ“‹ Company name fuzzy matching (Levenshtein distance)
â”œâ”€â”€ ðŸ“ Address and contact information matching
â”œâ”€â”€ ðŸª Trade name and brand matching
â””â”€â”€ ðŸ“ž Phone and email contact matching

Fuzzy Matching Algorithm:
â”œâ”€â”€ ðŸŽ¯ String similarity scoring (0-100%)
â”œâ”€â”€ ðŸ“Š Weighted feature matching (CNPJ=50%, Name=30%, Address=20%)
â”œâ”€â”€ ðŸ” Threshold-based decision making (>80% = match)
â”œâ”€â”€ ðŸ‘¥ Human review queue for 60-80% matches
â””â”€â”€ ðŸ†• Auto-suggest new supplier creation for <60%

New Supplier Creation:
â”œâ”€â”€ ðŸ“‹ Pre-populated supplier form with extracted data
â”œâ”€â”€ âœ… Mandatory field validation and completion
â”œâ”€â”€ ðŸ” Duplicate prevention checks
â”œâ”€â”€ ðŸ“Š Credit check and risk assessment initiation
â””â”€â”€ ðŸŽ¯ Workflow routing for approval based on supplier size
```

### **ðŸ“¦ Product/Ingredient Mapping**
```
Product Identification:
â”œâ”€â”€ ðŸ·ï¸ SKU/barcode exact matching
â”œâ”€â”€ ðŸ“‹ Product name fuzzy matching
â”œâ”€â”€ ðŸ“ Unit of measure compatibility
â”œâ”€â”€ ðŸª Supplier-specific product codes
â””â”€â”€ ðŸ“Š Historical purchase pattern matching

Intelligent Suggestions:
â”œâ”€â”€ ðŸŽ¯ ML-based product recommendations
â”œâ”€â”€ ðŸ“Š Purchase history analysis
â”œâ”€â”€ ðŸ”— Category-based suggestions
â”œâ”€â”€ ðŸ’° Price range validation
â””â”€â”€ ðŸ“ˆ Trending product identification

New Product Creation Workflow:
â”œâ”€â”€ ðŸ“‹ Product master data form pre-population
â”œâ”€â”€ ðŸ·ï¸ Category suggestion based on description
â”œâ”€â”€ ðŸ“ Unit of measure detection and validation
â”œâ”€â”€ ðŸ’° Price reasonableness validation
â””â”€â”€ ðŸ“Š Integration with product catalog management
```

## ðŸ“Š Performance and Quality Metrics

### **âš¡ Processing Performance**
```
Speed Targets:
â”œâ”€â”€ ðŸ“„ Document upload: < 30 seconds
â”œâ”€â”€ ðŸ”¤ OCR processing: < 2 minutes
â”œâ”€â”€ ðŸ§  ML extraction: < 3 minutes
â”œâ”€â”€ ðŸ‘¤ Validation queue time: < 4 hours
â””â”€â”€ ðŸ”— Entity mapping: < 1 minute

Throughput Targets:
â”œâ”€â”€ ðŸ“Š 100+ documents per hour (peak processing)
â”œâ”€â”€ ðŸ‘¥ 10+ concurrent validation sessions
â”œâ”€â”€ ðŸŽ¯ 1000+ documents per day capacity
â””â”€â”€ ðŸ“ˆ 99.5% uptime for AI services

Quality Targets:
â”œâ”€â”€ ðŸŽ¯ OCR accuracy: > 95% character accuracy
â”œâ”€â”€ ðŸ§  ML extraction: > 85% field accuracy
â”œâ”€â”€ ðŸ‘¤ Human validation: > 98% final accuracy
â””â”€â”€ ðŸ”— Entity mapping: > 90% auto-match rate
```

### **ðŸ“ˆ Business Impact Metrics**
```
Automation Benefits:
â”œâ”€â”€ â±ï¸ Time savings: 80% reduction in manual data entry
â”œâ”€â”€ ðŸŽ¯ Accuracy improvement: 95%+ vs 85% manual entry
â”œâ”€â”€ ðŸ’° Cost reduction: 60% lower processing costs
â””â”€â”€ ðŸ“Š Throughput increase: 300% more documents processed

User Experience Metrics:
â”œâ”€â”€ ðŸ˜Š User satisfaction: > 4.5/5 rating
â”œâ”€â”€ ðŸŽ¯ Task completion rate: > 95%
â”œâ”€â”€ â±ï¸ Learning curve: < 2 hours to proficiency
â””â”€â”€ ðŸ”„ Feature adoption rate: > 80% regular usage

Business Process Metrics:
â”œâ”€â”€ ðŸ“‹ Purchase order creation time: 70% reduction
â”œâ”€â”€ ðŸŽ¯ Supplier onboarding speed: 50% faster
â”œâ”€â”€ ðŸ’° Error-related costs: 80% reduction
â””â”€â”€ ðŸ“Š Compliance accuracy: > 99% regulatory compliance
```

## ðŸ”§ Error Handling and Recovery

### **âŒ Common Error Scenarios**
```
Document Processing Errors:
â”œâ”€â”€ ðŸ“„ Corrupted or unreadable files
â”œâ”€â”€ ðŸ”¤ OCR failure on poor quality images
â”œâ”€â”€ ðŸ§  ML model confidence below thresholds
â”œâ”€â”€ ðŸ“Š Inconsistent or contradictory data extraction
â””â”€â”€ ðŸ”— Entity mapping failures

Technical Infrastructure Errors:
â”œâ”€â”€ ðŸ¤– AI service unavailability
â”œâ”€â”€ ðŸ’¾ Storage system failures
â”œâ”€â”€ ðŸ”Œ Network connectivity issues
â”œâ”€â”€ ðŸ“Š Database transaction failures
â””â”€â”€ ðŸ”„ Queue processing bottlenecks

Business Logic Errors:
â”œâ”€â”€ âœ… Validation rule violations
â”œâ”€â”€ ðŸ¢ Supplier data inconsistencies
â”œâ”€â”€ ðŸ“¦ Product catalog mismatches
â”œâ”€â”€ ðŸ’° Financial calculation errors
â””â”€â”€ ðŸ“… Date and timeline logical conflicts
```

### **ðŸ”„ Recovery Mechanisms**
```
Automatic Recovery:
â”œâ”€â”€ ðŸ” Retry mechanisms with exponential backoff
â”œâ”€â”€ ðŸŽ¯ Fallback AI engines for processing failures
â”œâ”€â”€ ðŸ’¾ Data backup and restoration procedures
â”œâ”€â”€ ðŸ”„ Queue redistribution for load balancing
â””â”€â”€ ðŸ“Š Health check monitoring and auto-scaling

Manual Recovery:
â”œâ”€â”€ ðŸ‘¤ Human operator intervention workflows
â”œâ”€â”€ ðŸ“‹ Manual processing fallback procedures
â”œâ”€â”€ ðŸ” Error investigation and resolution tools
â”œâ”€â”€ ðŸ“Š Data correction and reprocessing capabilities
â””â”€â”€ ðŸ“ˆ Root cause analysis and prevention measures

Quality Assurance:
â”œâ”€â”€ âœ… Multi-stage validation and verification
â”œâ”€â”€ ðŸ“Š Cross-reference validation with external sources
â”œâ”€â”€ ðŸŽ¯ Confidence threshold management
â”œâ”€â”€ ðŸ‘¥ Peer review for critical documents
â””â”€â”€ ðŸ“‹ Audit trail maintenance for compliance
```

---

**Arquivo**: `04-ai-processing-flow.md`  
**Fluxo**: IA Processing (Fiscal Document Upload â†’ Purchase Order Creation)  
**DomÃ­nio**: Purchasing (with AI/ML integration)  
**Complexidade**: ðŸš¨ Alta (10+ participantes, 25+ interaÃ§Ãµes, AI pipeline)  
**AtualizaÃ§Ã£o**: 16/06/2025
